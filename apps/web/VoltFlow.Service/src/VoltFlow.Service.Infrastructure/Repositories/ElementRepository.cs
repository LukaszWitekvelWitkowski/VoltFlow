using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Exceptions;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Element.DTOs;
using VoltFlow.Service.Core.Models.Element.Request;
using VoltFlow.Service.Core.Pagination;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class ElementRepository : CacheRepository<ElementCacheDTO, ElementDTO, Element>, IElementRepository
    {
        public ElementRepository(VoltFlowDbContext context, IConfiguration configuration) : base(context, configuration) 
        { }
  
      
        public async Task<ElementDTO> AddElement(CreateElementRequest request)
        {

            var elemntGroupExists = await _context.Set<ElementGroup>()
                .AsNoTracking()
                .AnyAsync(eg => eg.IdElementGroup == request.ElementGroupId);

            if (!elemntGroupExists)
                throw new NotFoundException("Nie znaleziono grupy elementów o podanym Id.");

            var newElement = new Element
            {
                Name = request.Name.Trim(),
                Description = request.Description,
                ElementGroupId = request.ElementGroupId,
                IsObsolete = false
            };

            _context.Set<Element>().Add(newElement);
            await _context.SaveChangesAsync();

            _cache = null;

            return MapToDto(newElement);
        }

        public async Task<ElementCacheDTO> GetElementsQuery()
        {
            var cache = await GetOrUpdateCacheAsync();

            if (cache != null)
                return cache;

            var dbData = await FetchFromDbInternal();
            return new ElementCacheDTO { Items = dbData };
        }

        public async Task<ElementDTO?> GetElementByIdQuery(int id)
        {
            var cache = await GetOrUpdateCacheAsync();
            if (cache != null)
            {
                return cache.Items.FirstOrDefault(e => e.IdElement == id);
            }

            return await _context.Set<Element>()
                .AsNoTracking()
                .AsSplitQuery()
                .Where(e => e.IdElement == id)
                .Select(e => MapToDto(e))
                .FirstOrDefaultAsync();

        }

       
        override
        public ElementDTO MapToDto(Element e) => new ElementDTO
        {
            IdElement = e.IdElement,
            Name = e.Name,
            Description = e.Description,
            IsObsolete = e.IsObsolete,
            ElementGroupId = e.ElementGroupId
        };


        public async Task<PagedResultDTO<ElementDTO>> GetElementsPagedByNameQuery(string? name, int page, int size)
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

   
            var dbQuery = _context.Set<Element>().AsNoTracking().AsSplitQuery();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var search = name.Trim().ToLower();
                dbQuery = dbQuery.Where(e => e.Name.ToLower().Contains(search));
            }

            var dbTotalCount = await dbQuery.CountAsync();
            var dbItems = await dbQuery
                .OrderBy(e => e.Name)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(e => MapToDto(e))
                .ToListAsync();

            return new PagedResultDTO<ElementDTO>(dbItems, dbTotalCount, page, size);
        }

        public async Task<ElementDTO> UpdateElement(UpdateElementRequest request)
        {
            // 1. Aktualizacja w bazie
            var element = await _context.Set<Element>()
                .AsSplitQuery()
                .FirstOrDefaultAsync(e => e.IdElement == request.Id);

            if (element == null)
                throw new NotFoundException("Nie znaleziono elementu do aktualizacji.");

            element.Name = request.Name.Trim();
            element.IsObsolete = request.IsObsolete;
            element.ElementGroupId = request.ElementGroupId;
            element.Description = request.Description;

            await _context.SaveChangesAsync();

            ResetStaticCache();

            return MapToDto(element);
        }

        public async Task<bool> IsExists(string name, int? id = null)
        {
            var cache = await GetOrUpdateCacheAsync();
            var normalizedName = name?.Trim().ToLower() ?? string.Empty;

            if (cache != null)
            {
                // Sprawdzenie w zbuforowanej liście
                return cache.Items.Any(e => (id == null || e.IdElement != id)
                                               && e.Name.ToLower() == normalizedName);
            }

            return await _context.Set<Element>()
                .AsNoTracking()
                .AsSplitQuery()
                .AnyAsync(e => (id == null || e.IdElement != id)
                               && e.Name.ToLower() == normalizedName);
        }
    }
 }
