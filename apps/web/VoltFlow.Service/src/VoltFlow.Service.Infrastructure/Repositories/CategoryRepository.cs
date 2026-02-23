using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Exceptions;
using VoltFlow.Service.Core.Models.Category.DTOs;
using VoltFlow.Service.Core.Models.Category.Request;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Pagination;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class CategoryRepository : CacheRepository<CategoryCacheDTO,CategoryDTO, Category>, ICategoryRepository
    {
        public CategoryRepository(VoltFlowDbContext context, IConfiguration configuration)
        : base(context, configuration)
        {
        }

        public async Task<CategoryCacheDTO> GetCategoriesQuery()
        {


            var cache = await GetOrUpdateCacheAsync();
            if (cache != null) return cache;

            var dbData = await FetchFromDbInternal();
            return new CategoryCacheDTO() { Items = dbData };
        }

        public async Task<CategoryDTO> GetCategoryByIdQuery(int id)
        {
            var cache = await GetOrUpdateCacheAsync();
            if (cache != null)
            {
                var item = cache.Items.FirstOrDefault(c => c.Id == id);

                // ZAMIAST: return ServiceResponse<CategoryDTO>.Result(item!);
                if (item == null) throw new NotFoundException($"Kategoria o ID {id} nie istnieje.");

                return item;
            }

            var category = await _context.Set<Category>()
                .AsNoTracking()
                .Where(c => c.IdCategory == id)
                .Select(c => MapToDto(c))
                .FirstOrDefaultAsync();

            // TUTAJ RÓWNIEŻ:
            if (category == null) throw new NotFoundException($"Kategoria o ID {id} nie istnieje.");

            return category;
        }

        public async Task<PagedResultDTO<CategoryDTO>> GetCategoriesPagedByNameQuery(string? name, int page, int size)
        {
            var cache = await GetOrUpdateCacheAsync();

            if (cache != null)
            {
                return PagedHelper.ToPagedResponse(cache, name, c => c.Name, page, size);
            }

            // Fallback SQL
            var dbQuery = _context.Set<Category>().AsNoTracking();
            if (!string.IsNullOrWhiteSpace(name))
            {
                var search = name.Trim().ToLower();
                dbQuery = dbQuery.Where(c => c.Name.ToLower().Contains(search));
            }

            var dbTotal = await dbQuery.CountAsync();
            var dbItems = await dbQuery.OrderBy(c => c.Name)
                .Skip((page - 1) * size).Take(size)
                .Select(c => MapToDto(c)).ToListAsync();

            return new PagedResultDTO<CategoryDTO>(dbItems, dbTotal, page, size);
        }

        public async Task<CategoryDTO> AddCategory(string name)
        {
            var newCategory = new Category
            {
                Name = name.Trim(),
                IsObsolete = false
            };

            _context.Set<Category>().Add(newCategory);
            await _context.SaveChangesAsync();

            ResetStaticCache();

            return MapToDto(newCategory);
        }

        public async Task<CategoryDTO> UpdateCategory(UpdateCategoryRequest request)
        {
            var category = await _context.Set<Category>()
                .FirstOrDefaultAsync(c => c.IdCategory == request.Id);

            if (category == null)
                throw new NotFoundException("Kategoria nie istnieje.");

            category.Name = request.Name.Trim();
            category.IsObsolete = request.IsObsolete;

            await _context.SaveChangesAsync();

            ResetStaticCache();

            return MapToDto(category);
        }

        public async Task<bool> IsExists(string name, int? id = null)
        {
            var cache = await GetOrUpdateCacheAsync();
            var normalizedName = name?.Trim().ToLower() ?? string.Empty;

            if (cache != null)
            {
                return cache.Items.Any(c => (id == null || c.Id != id)
                                                 && c.Name.ToLower() == normalizedName);
            }

            return await _context.Set<Category>()
                .AnyAsync(c => (id == null || c.IdCategory != id)
                                && c.Name.ToLower() == normalizedName);
        }

        override
        public  CategoryDTO MapToDto(Category c) => new CategoryDTO
        {
            Id = c.IdCategory,
            Name = c.Name,
            IsObsolete = c.IsObsolete
        };

        public async Task<CategoryDTO?> CategoryExistsByName(string name)
        {
            var cache = await GetOrUpdateCacheAsync();
            var normalizedName = name?.Trim().ToLower() ?? string.Empty;

            if (cache != null)
            {
                // Szukamy w pamięci RAM (szybka operacja)
                return cache.Items
                    .FirstOrDefault(c => c.Name.ToLower() == normalizedName);
            }

            // Fallback do bazy danych, jeśli cache jest wyłączony
            return await _context.Set<Category>()
                .AsNoTracking()
                .Where(c => c.Name.ToLower() == normalizedName)
                .Select(c => MapToDto(c))
                .FirstOrDefaultAsync();
        }
    }
}