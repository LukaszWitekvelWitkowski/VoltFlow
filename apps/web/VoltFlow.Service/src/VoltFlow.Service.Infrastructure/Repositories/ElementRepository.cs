using Microsoft.EntityFrameworkCore;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Element.DTOs;
using VoltFlow.Service.Core.Pagination;
using VoltFlow.Service.Core.Tools;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class ElementRepository : IElementRepository
    {
        private readonly VoltFlowDbContext _context;
        private readonly LazyValue<ServiceResponse<ElementsDTO>> _allElementsLazy;
        public ElementRepository(VoltFlowDbContext context)
        {
            _context = context;
            _allElementsLazy = new LazyValue<ServiceResponse<ElementsDTO>>(FetchElementsFromDb);
        }
        public async Task<ServiceResponse<ElementDTO>> GetElementByIdQuery(int id)
        {
            var response = await _allElementsLazy.GetValueAsync();

            if (!response._IsSuccess)
            {
                return ServiceResponse<ElementDTO>.Failure(response._Message, response._StatusCode);
            }

            var element = response._Data?.Elements.FirstOrDefault(e => e.IdElement == id);

            return ServiceResponse<ElementDTO>.Result(element);
        }

        public async Task<ServiceResponse<PagedResultDTO<ElementDTO>>> GetElementsPagedByNameQuery(string? name, int page, int size)
        {
            var response = await _allElementsLazy.GetValueAsync();

            if (!response._IsSuccess)
            {
                return ServiceResponse<PagedResultDTO<ElementDTO>>.Failure(response._Message, response._StatusCode);
            }

            var listResponse = ServiceResponse<List<ElementDTO>>.Result(response._Data?.Elements ?? new List<ElementDTO>());

            return PagedHelper.ToPagedResponse(
                listResponse,
                name,
                e => e.Name,
                page,
                size
            );
        }

        public async Task<ServiceResponse<ElementsDTO>> GetElementsQuery()
        {
            var fullResponse = await _allElementsLazy.GetValueAsync();

            return ServiceResponse<ElementsDTO>.Result(fullResponse._Data);
        }

        private async Task<ServiceResponse<ElementsDTO>> FetchElementsFromDb()
        {
            try
            {
                var elementList = await _context.Set<Element>()
                    .AsNoTracking() // Złota zasada Seniora: Odczyt do DTO = AsNoTracking (wydajność!)
                    .Select(e => new ElementDTO
                    {
                        IdElement = e.IdElement,
                        Name = e.Name,
                        Description = e.Description,
                        IsObsolete = e.IsObsolete,
                        ElementGroupId = e.ElementGroupId
                    }).ToListAsync();

                return ServiceResponse<ElementsDTO>.Result(new ElementsDTO { Elements = elementList });
            }
            catch (Exception ex)
            {
                // Logowanie błędu tutaj (np. ILogger)
                return ServiceResponse<ElementsDTO>.Failure("Błąd podczas pobierania danych z bazy.", 500);
            }
        }
    }
}
