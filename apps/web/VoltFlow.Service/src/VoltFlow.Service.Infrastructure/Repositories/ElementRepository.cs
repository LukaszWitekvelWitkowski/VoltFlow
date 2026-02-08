using Microsoft.EntityFrameworkCore;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Element.DTOs;
using VoltFlow.Service.Core.Models.Element.Request;
using VoltFlow.Service.Core.Pagination;
using VoltFlow.Service.Core.Tools;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class ElementRepository : IElementRepository
    {
        private readonly VoltFlowDbContext _context;
        private  LazyValue<ServiceResponse<ElementsDTO>> _allElementsLazy;
        public ElementRepository(VoltFlowDbContext context)
        {
            _context = context;
            _allElementsLazy = new LazyValue<ServiceResponse<ElementsDTO>>(FetchElementsFromDb);
        }

        public async Task<ServiceResponse<ElementDTO>> AddElement(string name)
        {
            try
            {
  
                var newElement = new Element
                {
                    Name = name,
                    IsObsolete = false,
                };

                _context.Set<Element>().Add(newElement);
                await _context.SaveChangesAsync();


                _allElementsLazy = new LazyValue<ServiceResponse<ElementsDTO>>(FetchElementsFromDb);


                var allElementsResponse = await _allElementsLazy.GetValueAsync();
                var elementDTO = allElementsResponse._Data?.Elements
                    .FirstOrDefault(e => e.IdElement == newElement.IdElement);

                return ServiceResponse<ElementDTO>.Result(elementDTO!);
            }
            catch (Exception ex)
            {
                return ServiceResponse<ElementDTO>.Failure("Nie udało się zapisać elementu w bazie danych.", 500);
            }
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

            var listResponse = ServiceResponse<List<ElementDTO>>.Result((List<ElementDTO>?)(response._Data?.Elements ?? new List<ElementDTO>()));

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

        public async Task<ServiceResponse<ElementDTO>> UpdateElement(UpdateElementRequest request)
        {
            try
            {
                var element = await _context.Set<Element>()
                    .FirstOrDefaultAsync(e => e.IdElement == request.Id);

                if (element == null)
                {
                    return ServiceResponse<ElementDTO>.Failure("Nie znaleziono elementu o podanym ID.", 404);
                }

                element.Name = request.Name.Trim();
                element.IsObsolete = request.IsObsolete;

                await _context.SaveChangesAsync();

                _allElementsLazy = new LazyValue<ServiceResponse<ElementsDTO>>(FetchElementsFromDb);


                return ServiceResponse<ElementDTO>.Result(new ElementDTO
                {
                    IdElement = element.IdElement,
                    Name = element.Name,
                    IsObsolete = element.IsObsolete,
                    Description = element.Description, // Przekazujemy istniejące dane
                    ElementGroupId = element.ElementGroupId
                });
            }
            catch (Exception ex)
            {
                return ServiceResponse<ElementDTO>.Failure("Wystąpił błąd bazy danych podczas aktualizacji elementu.", 500);
            }
        }

        private async Task<ServiceResponse<ElementsDTO>> FetchElementsFromDb()
        {
            try
            {
                var elementList = await _context.Set<Element>()
                    .AsNoTracking() 
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
         
                return ServiceResponse<ElementsDTO>.Failure("Błąd podczas pobierania danych z bazy.", 500);
            }
        }
    }
}
