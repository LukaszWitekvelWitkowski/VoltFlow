using Microsoft.EntityFrameworkCore;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models;
using VoltFlow.Service.Core.Models.Element;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class ElementRepository : IElementRepository
    {
        private readonly VoltFlowDbContext _context;
        public ElementRepository(VoltFlowDbContext context)
        {
            _context = context;
        }
        public async Task<ServiceResponse<ElementDTO>> GetElementByIdQuery(int id)
        {
            var element = await _context.Set<Element>()
                .Where(e => e.IdElement == id)
                .Select(e => new ElementDTO
                {
                    IdElement = e.IdElement,
                    Name = e.Name,
                    Description = e.Description,
                    IsObsolete = e.IsObsolete,
                    ElementGroupId = e.ElementGroupId
                }).FirstOrDefaultAsync();

            return ServiceResponse<ElementDTO>.Result(element);
        }

        public async Task<ServiceResponse<ElementDTO>> GetElementByNameQuery(string name)
        {
            var element = await _context.Set<Element>()
                    .Where(e => e.Name == name)
                    .Select(e => new ElementDTO
                    {
                        IdElement = e.IdElement,
                        Name = e.Name,
                        Description = e.Description,
                        IsObsolete = e.IsObsolete,
                        ElementGroupId = e.ElementGroupId
                    }).FirstOrDefaultAsync();

            return ServiceResponse<ElementDTO>.Result(element);
        }

        public Task<ServiceResponse<ElementsDTO>> GetElementsQuery()
        {
           var elementList = _context.Set<Element>()
                .Select(e => new ElementDTO
                {
                    IdElement = e.IdElement,
                    Name = e.Name,
                    Description = e.Description,
                    IsObsolete = e.IsObsolete,
                    ElementGroupId = e.ElementGroupId
                }).ToList();

            var elementsDTO = new ElementsDTO { Elements = elementList };

            return Task.FromResult(ServiceResponse<ElementsDTO>.Result(elementsDTO));
        }
    }
}
