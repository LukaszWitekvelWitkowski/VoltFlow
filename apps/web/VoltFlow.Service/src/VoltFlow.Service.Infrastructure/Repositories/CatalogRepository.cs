using Microsoft.EntityFrameworkCore;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Catalog.DTOs;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Requests;
using VoltFlow.Service.Core.Pagination;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly VoltFlowDbContext _context;

        public CatalogRepository(VoltFlowDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<PagedResultDTO<ElementTreeDTO>>> GetCatalogSearchQuery(CatalogSearchRequest request)
        {
            try
            {
                var query = _context.Set<Element>()
                    .AsNoTracking()
                    .Include(e => e.ElementGroup)
                        .ThenInclude(eg => eg.Category)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.ElementName))
                {
                    query = query.Where(e => e.Name.Contains(request.ElementName));
                }

                var totalCount = await query.CountAsync();

                var items = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(e => new ElementTreeDTO
                    {
                        Id = e.IdElement,
                        Name = e.Name,
                        Group = new ElementGroupSimpleDTO
                        {
                            Id = e.ElementGroup.IdElementGroup,
                            Name = e.ElementGroup.Name,
                            Category = new CategorySimpleDTO
                            {
                                Id = e.ElementGroup.Category.IdCategory,
                                Name = e.ElementGroup.Category.Name
                            }
                        }
                    })
                    .ToListAsync();

                // 1. Najpierw filtrujemy listę wejściową na podstawie requestu
                var filteredItems = items.AsEnumerable();

                if (request != null)
                {
                    // Filtracja po Kategorii
                    if (request.CategoryId.HasValue)
                        filteredItems = filteredItems.Where(e => e.Group?.Category?.Id == request.CategoryId);

                    if (!string.IsNullOrWhiteSpace(request.CategoryName))
                        filteredItems = filteredItems.Where(e => e.Group?.Category?.Name.Contains(request.CategoryName, StringComparison.OrdinalIgnoreCase) == true);

                    // Filtracja po Grupie
                    if (request.ElementGroupId.HasValue)
                        filteredItems = filteredItems.Where(e => e.Group?.Id == request.ElementGroupId);

                    if (!string.IsNullOrWhiteSpace(request.ElementGroupName))
                        filteredItems = filteredItems.Where(e => e.Group?.Name.Contains(request.ElementGroupName, StringComparison.OrdinalIgnoreCase) == true);

                    // Filtracja po Elemencie (liściu)
                    if (request.ElementId.HasValue)
                        filteredItems = filteredItems.Where(e => e.Id == request.ElementId);

                    if (!string.IsNullOrWhiteSpace(request.ElementName))
                        filteredItems = filteredItems.Where(e => e.Name.Contains(request.ElementName, StringComparison.OrdinalIgnoreCase) == true);
                }

                // 2. Dopiero przefiltrowaną listę przekazujemy do DTO


                var pagedResult = new PagedResultDTO<ElementTreeDTO>
                {
                    Relusts = filteredItems.ToList() ?? new List<ElementTreeDTO>(),
                    TotalCount = totalCount,
                };


                return ServiceResponse<PagedResultDTO<ElementTreeDTO>>.Result(pagedResult);
            }
            catch (Exception ex)
            {
                return ServiceResponse<PagedResultDTO<ElementTreeDTO>>.Failure("Błąd podczas pobierania drzewa elementów.", 500);
            }
        }
    }
}
