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

      
        public async Task<ServiceResponse<ElementGroupDTO>> AddElementGroup(CreateElementGroupRequest request)
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

            return ServiceResponse<ElementGroupDTO>.Success(MapToDto(newGroup));
        }

        public async Task<ServiceResponse<ElementGroupDTO>> UpdateElementGroup(UpdateElementGroupRequest request)
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

            return ServiceResponse<ElementGroupDTO>.Success(MapToDto(elementGroup));
        }

        public async Task<ServiceResponse<ElementGroupDTO>> GetElementGroupByIdQuery(int id)
        {
            var cache = await GetOrUpdateCacheAsync();
            if (cache != null)
            {
                var item = cache.Items.FirstOrDefault(eg => eg.IdElementGroup == id);
                if (item == null) throw new NotFoundException($"Grupa elementów o ID {id} nie istnieje.");

                return ServiceResponse<ElementGroupDTO>.Result(item);
            }

            var group = await _context.Set<ElementGroup>()
                .AsNoTracking()
                .Where(eg => eg.IdElementGroup == id)
                .Select(eg => MapToDto(eg))
                .FirstOrDefaultAsync();

            return ServiceResponse<ElementGroupDTO>.Result(group!);
        }

        public async Task<ServiceResponse<PagedResultDTO<ElementGroupDTO>>> GetElementGroupSearchQuery(string? name, int page, int size)
        {
            var cache = await GetOrUpdateCacheAsync();

            if (cache != null)
            {
                // Używamy PagedHelper dla danych z cache
                var sourceResponse = ServiceResponse<IEnumerable<ElementGroupDTO>>.Result(cache.Items);

                return PagedHelper.ToPagedResponse(
                    sourceResponse,
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

            return ServiceResponse<PagedResultDTO<ElementGroupDTO>>.Result(
                new PagedResultDTO<ElementGroupDTO>(dbItems, dbTotal, page, size));
        }

        public async Task<ServiceResponse<ElementGroupCacheDTO>> GetElementGroupsQuery()
        {
            var cache = await GetOrUpdateCacheAsync();
            if (cache != null) return ServiceResponse<ElementGroupCacheDTO>.Result(cache);

            var data = await FetchFromDbInternal();
            return ServiceResponse<ElementGroupCacheDTO>.Result(new ElementGroupCacheDTO() {Items = data });
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
