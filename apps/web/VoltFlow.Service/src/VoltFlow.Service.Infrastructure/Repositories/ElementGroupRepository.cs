using Microsoft.EntityFrameworkCore;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;
using VoltFlow.Service.Core.Pagination;
using VoltFlow.Service.Core.Tools;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class ElementGroupRepository : IElementGroupRepository
    {
        private readonly VoltFlowDbContext _context;
        private readonly LazyValue<ServiceResponse<ElementGroupsDTO>> _allElementGroupLazy;

        public ElementGroupRepository(VoltFlowDbContext context)
        {
            _context = context;

            _allElementGroupLazy = new LazyValue<ServiceResponse<ElementGroupsDTO>>(FetchElementGroupsFromDb);
        }

        public async Task<ServiceResponse<ElementGroupsDTO>> GetElementGroupsQuery()
        {
            var response = await _allElementGroupLazy.GetValueAsync();

            return ServiceResponse<ElementGroupsDTO>.Result(response._Data);
        }

        public async Task<ServiceResponse<ElementGroupDTO>> GetElementGroupByIdQuery(int id)
        {
            var response = await _allElementGroupLazy.GetValueAsync();

            if (!response._IsSuccess)
            {
                return ServiceResponse<ElementGroupDTO>.Failure(response._Message, response._StatusCode);
            }

            var elementGroup = response._Data?.ElementGroups.FirstOrDefault(eg => eg.IdElementGroup == id);

            return ServiceResponse<ElementGroupDTO>.Result(elementGroup);
        }

        public async Task<ServiceResponse<PagedResultDTO<ElementGroupDTO>>> GetElementGroupSearchQuery(string? name, int page, int size)
        {
            var response = await _allElementGroupLazy.GetValueAsync();

            if (!response._IsSuccess)
            {
                return ServiceResponse<PagedResultDTO<ElementGroupDTO>>.Failure(response._Message, response._StatusCode);
            }

            var listResponse = ServiceResponse<List<ElementGroupDTO>>.Result(
                response._Data?.ElementGroups.ToList() ?? new List<ElementGroupDTO>()
            );

            return PagedHelper.ToPagedResponse(
                listResponse,
                name,
                eg => eg.Name, // Filtrowanie po nazwie
                page,
                size
            );
        }

        private async Task<ServiceResponse<ElementGroupsDTO>> FetchElementGroupsFromDb()
        {
            try
            {
                var groupsList = await _context.Set<ElementGroup>()
                    .AsNoTracking()
                    .Select(eg => new ElementGroupDTO
                    {
                        IdElementGroup = eg.IdElementGroup,
                        Name = eg.Name,
                        IsObsolete = eg.IsObsolete,
                        CategoryId = eg.CategoryId
                    })
                    .ToListAsync();

                return ServiceResponse<ElementGroupsDTO>.Result(new ElementGroupsDTO(groupsList));
            }
            catch (Exception ex)
            {
         
                return ServiceResponse<ElementGroupsDTO>.Failure("Błąd podczas pobierania grup elementów z bazy.", 500);
            }
        }
    }
}
