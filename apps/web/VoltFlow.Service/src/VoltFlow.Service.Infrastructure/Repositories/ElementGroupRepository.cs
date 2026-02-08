using Microsoft.EntityFrameworkCore;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;
using VoltFlow.Service.Core.Models.ElementGroup.Request;
using VoltFlow.Service.Core.Pagination;
using VoltFlow.Service.Core.Tools;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class ElementGroupRepository : IElementGroupRepository
    {
        private readonly VoltFlowDbContext _context;
        private LazyValue<ServiceResponse<ElementGroupsDTO>> _allElementGroupLazy;

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
                eg => eg.Name,
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

        public async Task<ServiceResponse<ElementGroupDTO>> AddElementGroup(CreateElementGroupRequest request)
        {
            try
            {
                var category = await _context.Set<Category>()
                        .FirstAsync(c => c.IdCategory == request.CategoryId);
    
                var newGroup = new ElementGroup
                {
                    Name = request.Name.Trim(),
                    CategoryId = request.CategoryId, 
                    IsObsolete = false,
                    Category = category
                };

                _context.Set<ElementGroup>().Add(newGroup);
                await _context.SaveChangesAsync();

                _allElementGroupLazy = new LazyValue<ServiceResponse<ElementGroupsDTO>>(FetchElementGroupsFromDb);

                var allGroupsResponse = await _allElementGroupLazy.GetValueAsync();
                var groupDTO = allGroupsResponse._Data?.ElementGroups
                    .FirstOrDefault(g => g.IdElementGroup == newGroup.IdElementGroup);

                return ServiceResponse<ElementGroupDTO>.Result(groupDTO!);
            }
            catch (Exception ex)
            {
                return ServiceResponse<ElementGroupDTO>.Failure("Nie udało się zapisać grupy elementów w bazie danych.", 500);
            }
        }

        public async Task<ServiceResponse<ElementGroupDTO>> UpdateElementGroup(UpdateElementGroupRequest request)
        {
            try
            {
                var elementGroup = await _context.Set<ElementGroup>()
                    .FirstOrDefaultAsync(eg => eg.IdElementGroup == request.IdElementGroup);

                if (elementGroup == null)
                {
                    return ServiceResponse<ElementGroupDTO>.Failure("Nie znaleziono grupy elementów o podanym ID.", 404);
                }


                var category = await _context.Set<Category>()
                    .FirstOrDefaultAsync(c => c.IdCategory == request.CategoryId);

                if (category == null)
                {
                    return ServiceResponse<ElementGroupDTO>.Failure("Nie znaleziono kategorii o podanym ID.", 404);
                }


                elementGroup.Name = request.Name.Trim();
                elementGroup.IsObsolete = request.IsObsolete;
                elementGroup.CategoryId = request.CategoryId;
                elementGroup.Category = category; 
     
                await _context.SaveChangesAsync();

      
                _allElementGroupLazy = new LazyValue<ServiceResponse<ElementGroupsDTO>>(FetchElementGroupsFromDb);


                return ServiceResponse<ElementGroupDTO>.Result(new ElementGroupDTO
                {
                    IdElementGroup = elementGroup.IdElementGroup,
                    Name = elementGroup.Name,
                    IsObsolete = elementGroup.IsObsolete,
                    CategoryId = elementGroup.CategoryId
                });
            }
            catch (Exception ex)
            {
                return ServiceResponse<ElementGroupDTO>.Failure("Wystąpił błąd bazy danych podczas aktualizacji grupy elementów.", 500);
            }
        }
    }
}
