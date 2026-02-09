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
    public class ElementRepository : BaseRepository, IElementRepository
    {
       
        private ElementsDTO? _cache;

        public ElementRepository(VoltFlowDbContext context, IConfiguration configuration) : base(context, configuration) 
        { }
  
        private async Task<ElementsDTO> GetOrUpdateCacheAsync()
        {
            if (!_isCacheEnabled) return null!;

            if (_cache != null) return _cache;

            await _lock.WaitAsync();
            try
            {
                if (_cache == null)
                {
                    var count = await _context.Set<Element>().CountAsync();
                    if (count > _maxCacheThreshold)
                    {
                        _isCacheEnabled = false; 
                        return null!;
                    }

                    var data = await FetchFromDbInternal();
                    _cache = new ElementsDTO { Elements = data };
                }
                return _cache;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<ServiceResponse<ElementDTO>> AddElement(CreateElementRequest request)
        {

            var elemntGroupExists = await _context.Set<ElementGroup>()
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

            return ServiceResponse<ElementDTO>.Success(MapToDto(newElement));
        }

        public async Task<ServiceResponse<ElementsDTO>> GetElementsQuery()
        {
            var cache = await GetOrUpdateCacheAsync();

            if (cache != null)
                return ServiceResponse<ElementsDTO>.Result(cache);

            var dbData = await FetchFromDbInternal();
            return ServiceResponse<ElementsDTO>.Result(new ElementsDTO { Elements = dbData });
        }

        public async Task<ServiceResponse<ElementDTO>> GetElementByIdQuery(int id)
        {
            var cache = await GetOrUpdateCacheAsync();
            if (cache != null)
            {
                var item = cache.Elements.FirstOrDefault(e => e.IdElement == id);
                return ServiceResponse<ElementDTO>.Result(item!);
            }

            var element = await _context.Set<Element>()
                .AsNoTracking()
                .Where(e => e.IdElement == id)
                .Select(e => MapToDto(e))
                .FirstOrDefaultAsync();

            return ServiceResponse<ElementDTO>.Result(element!);
        }

        private async Task<IEnumerable<ElementDTO>> FetchFromDbInternal()
        {
            return await _context.Set<Element>()
                .AsNoTracking()
                .Select(e => MapToDto(e))
                .ToListAsync();
        }

        private static ElementDTO MapToDto(Element e) => new ElementDTO
        {
            IdElement = e.IdElement,
            Name = e.Name,
            Description = e.Description,
            IsObsolete = e.IsObsolete,
            ElementGroupId = e.ElementGroupId
        };


        public async Task<ServiceResponse<PagedResultDTO<ElementDTO>>> GetElementsPagedByNameQuery(string? name, int page, int size)
        {
            var cache = await GetOrUpdateCacheAsync();

            if (cache != null)
            {
                var sourceResponse = ServiceResponse<IEnumerable<ElementDTO>>.Result(cache.Elements);

                return PagedHelper.ToPagedResponse(
                    sourceResponse,
                    name,
                    eg => eg.Name,
                    page,
                    size
                );
            }

   
            var dbQuery = _context.Set<Element>().AsNoTracking();

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

            var dbPagedResult = new PagedResultDTO<ElementDTO>(dbItems, dbTotalCount, page, size);

            return ServiceResponse<PagedResultDTO<ElementDTO>>.Result(dbPagedResult);
        }

        public async Task<ServiceResponse<ElementDTO>> UpdateElement(UpdateElementRequest request)
        {
            // 1. Aktualizacja w bazie
            var element = await _context.Set<Element>()
                .FirstOrDefaultAsync(e => e.IdElement == request.Id);

            if (element == null)
                throw new NotFoundException("Nie znaleziono elementu do aktualizacji.");

            element.Name = request.Name.Trim();
            element.IsObsolete = request.IsObsolete;
            element.ElementGroupId = request.ElementGroupId;
            element.Description = request.Description;

            await _context.SaveChangesAsync();

            // 2. Kluczowy moment: Inwalidacja cache
            // Po udanym zapisie ustawiamy cache na null, aby następny odczyt pobrał świeże dane
            await _lock.WaitAsync();
            try
            {
                _cache = null;
            }
            finally
            {
                _lock.Release();
            }

            return ServiceResponse<ElementDTO>.Success(MapToDto(element));
        }

        public async Task<bool> IsExists(string name, int? id = null)
        {
            var cache = await GetOrUpdateCacheAsync();
            var normalizedName = name?.Trim().ToLower() ?? string.Empty;

            if (cache != null)
            {
                // Sprawdzenie w zbuforowanej liście
                return cache.Elements.Any(e => (id == null || e.IdElement != id)
                                               && e.Name.ToLower() == normalizedName);
            }

            // Jeśli cache wyłączony - standardowe szybkie zapytanie SQL
            return await _context.Set<Element>()
                .AnyAsync(e => (id == null || e.IdElement != id)
                               && e.Name.ToLower() == normalizedName);
        }
    }
 }
