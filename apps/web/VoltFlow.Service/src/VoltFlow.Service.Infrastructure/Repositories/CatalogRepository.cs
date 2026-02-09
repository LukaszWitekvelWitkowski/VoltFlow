using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Catalog.DTOs;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Requests;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class CatalogRepository : BaseRepository, ICatalogRepository
    {
        public CatalogRepository(VoltFlowDbContext context, IConfiguration configuration)
            : base(context, configuration)
        {
        }

        public async Task<ServiceResponse<PagedResultDTO<ElementTreeDTO>>> GetCatalogSearchQuery(CatalogSearchRequest request)
        {
            // 1. Budujemy zapytanie (IQueryable - jeszcze nie jedzie do bazy)
            var query = _context.Set<Element>()
                .AsNoTracking()
                .AsQueryable();

            // 2. Filtrowanie PO STRONIE SQL
            if (request != null)
            {
                // Filtracja po Elemencie
                if (request.ElementId.HasValue)
                    query = query.Where(e => e.IdElement == request.ElementId);

                if (!string.IsNullOrWhiteSpace(request.ElementName))
                    query = query.Where(e => e.Name.ToLower().Contains(request.ElementName.ToLower()));

                // Filtracja po Grupie
                if (request.ElementGroupId.HasValue)
                    query = query.Where(e => e.ElementGroupId == request.ElementGroupId);

                if (!string.IsNullOrWhiteSpace(request.ElementGroupName))
                    query = query.Where(e => e.ElementGroup.Name.ToLower().Contains(request.ElementGroupName.ToLower()));

                // Filtracja po Kategorii
                if (request.CategoryId.HasValue)
                    query = query.Where(e => e.ElementGroup.CategoryId == request.CategoryId);

                if (!string.IsNullOrWhiteSpace(request.CategoryName))
                    query = query.Where(e => e.ElementGroup.Category.Name.ToLower().Contains(request.CategoryName.ToLower()));
            }

            // 3. Liczymy rekordy spełniające kryteria (w SQL)
            var totalCount = await query.CountAsync();

            // 4. Paginacja i Projekcja (Select) w jednym zapytaniu SQL
            // Projection (Select) sprawia, że Include/ThenInclude są zbędne - EF sam wygeneruje Joina
            var items = await query
                .OrderBy(e => e.Name)
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

            var pagedResult = new PagedResultDTO<ElementTreeDTO>(items, totalCount, request.PageNumber, request.PageSize);

            return ServiceResponse<PagedResultDTO<ElementTreeDTO>>.Result(pagedResult);
        }
    }
}

