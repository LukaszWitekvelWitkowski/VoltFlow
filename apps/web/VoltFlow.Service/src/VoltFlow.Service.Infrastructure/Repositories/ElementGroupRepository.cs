using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Exceptions;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;
using VoltFlow.Service.Core.Models.ElementGroup.Request;
using VoltFlow.Service.Core.Pagination;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class ElementGroupRepository : CacheRepository<ElementGroupCacheDTO, ElementGroupDTO, ElementGroup>, IElementGroupRepository
    {
        public ElementGroupRepository(VoltFlowDbContext context, IConfiguration configuration) : base(context, configuration)
        {
        }

      
        public async Task<ElementGroupDTO> AddElementGroup(CreateElementGroupRequest request)
        {
            // Sprawdzamy czy kategoria istnieje (szybki AnyAsync)
            var categoryExists = await _context.Set<Category>()
                .AnyAsync(c => c.IdCategory == request.CategoryId);

            if (!categoryExists)
                throw new NotFoundException("Nie znaleziono kategorii o podanym ID.");

            var newGroup = new ElementGroup
            {
                Name = request.Name.Trim(),
                CategoryId = request.CategoryId,
                IsObsolete = false
            };

            _context.Set<ElementGroup>().Add(newGroup);
            await _context.SaveChangesAsync();

            // Inwalidacja cache
            ResetStaticCache();

            return MapToDto(newGroup);
        }

        public async Task<ElementGroupDTO> UpdateElementGroup(UpdateElementGroupRequest request)
        {
            var elementGroup = await _context.Set<ElementGroup>()
                .FirstOrDefaultAsync(eg => eg.IdElementGroup == request.IdElementGroup);

            if (elementGroup == null)
                throw new NotFoundException("Nie znaleziono grupy elementów.");

            var categoryExists = await _context.Set<Category>()
                .AnyAsync(c => c.IdCategory == request.CategoryId);

            if (!categoryExists)
                throw new NotFoundException("Nie znaleziono kategorii o podanym ID.");

            elementGroup.Name = request.Name.Trim();
            elementGroup.IsObsolete = request.IsObsolete;
            elementGroup.CategoryId = request.CategoryId;

            await _context.SaveChangesAsync();

            // Inwalidacja cache
            ResetStaticCache();

            return MapToDto(elementGroup);
        }

        public async Task<ElementGroupDTO?> GetElementGroupByIdQuery(int id)
        {
            var cache = await GetOrUpdateCacheAsync();
            if (cache != null)
            {
                return cache.Items.FirstOrDefault(eg => eg.IdElementGroup == id);
            }

            return await _context.Set<ElementGroup>()
                .AsNoTracking()
                .Where(eg => eg.IdElementGroup == id)
                .Select(eg => MapToDto(eg))
                .FirstOrDefaultAsync();
        }

        public async Task<PagedResultDTO<ElementGroupDTO>> GetElementGroupSearchQuery(string? name, int page, int size)
        {
            var cache = await GetOrUpdateCacheAsync();

            if (cache != null)
            {
                return PagedHelper.ToPagedResponse(
                    cache,
                    name,
                    eg => eg.Name,
                    page,
                    size
                );
            }

            // Fallback do SQL
            var dbQuery = _context.Set<ElementGroup>().AsNoTracking();
            if (!string.IsNullOrWhiteSpace(name))
            {
                var search = name.Trim().ToLower();
                dbQuery = dbQuery.Where(eg => eg.Name.ToLower().Contains(search));
            }

            var dbTotal = await dbQuery.CountAsync();
            var dbItems = await dbQuery
                .OrderBy(eg => eg.Name)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(eg => MapToDto(eg))
                .ToListAsync();

            return new PagedResultDTO<ElementGroupDTO>(dbItems, dbTotal, page, size);
        }

        public async Task<ElementGroupCacheDTO> GetElementGroupsQuery()
        {
            var cache = await GetOrUpdateCacheAsync();
            if (cache != null) return cache;

            var data = await FetchFromDbInternal();
            return new ElementGroupCacheDTO() {Items = data };
        }

        public async Task<bool> IsExists(string name, int? id = null)
        {
            var cache = await GetOrUpdateCacheAsync();
            var normalizedName = name?.Trim().ToLower() ?? string.Empty;

            if (cache != null)
            {
                return cache.Items.Any(eg => (id == null || eg.IdElementGroup != id)
                                                     && eg.Name.ToLower() == normalizedName);
            }

            return await _context.Set<ElementGroup>()
                .AnyAsync(eg => (id == null || eg.IdElementGroup != id)
                                && eg.Name.ToLower() == normalizedName);
        }

      
        override
        public ElementGroupDTO MapToDto(ElementGroup eg) => new ElementGroupDTO
        {
            IdElementGroup = eg.IdElementGroup,
            Name = eg.Name,
            IsObsolete = eg.IsObsolete,
            CategoryId = eg.CategoryId
        };
    }
}
