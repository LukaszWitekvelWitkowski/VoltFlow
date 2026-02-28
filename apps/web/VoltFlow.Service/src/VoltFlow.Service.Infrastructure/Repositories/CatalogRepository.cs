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

        public async Task<PagedResultDTO<ElementTreeDTO>> GetCatalogSearchQuery(CatalogSearchRequest request)
        {

            var query = _context.Set<Element>()
                .AsNoTracking()
                .AsQueryable();


            if (request != null)
            {
                // Sprawdzamy raz, czy działamy na Postgresie, żeby nie powtarzać tego w każdym IF-ie
                bool isNpgsql = _context.Database.IsNpgsql();

                // Filtracja po Elementach
                if (request.ElementId.HasValue)
                    query = query.Where(e => e.IdElement == request.ElementId);

                if (!string.IsNullOrWhiteSpace(request.ElementName))
                {
                    query = isNpgsql
                        ? query.Where(e => EF.Functions.ILike(e.Name, $"%{request.ElementName}%"))
                        : query.Where(e => e.Name.ToLower().Contains(request.ElementName.ToLower()));
                }

                // Filtracja po Grupie
                if (request.ElementGroupId.HasValue)
                    query = query.Where(e => e.ElementGroupId == request.ElementGroupId);

                if (!string.IsNullOrWhiteSpace(request.ElementGroupName))
                {
                    query = isNpgsql
                        ? query.Where(e => EF.Functions.ILike(e.ElementGroup.Name, $"%{request.ElementGroupName}%"))
                        : query.Where(e => e.ElementGroup.Name.ToLower().Contains(request.ElementGroupName.ToLower()));
                }

                // Filtracja po kategorii
                if (request.CategoryId.HasValue)
                    query = query.Where(e => e.ElementGroup.CategoryId == request.CategoryId);

                if (!string.IsNullOrWhiteSpace(request.CategoryName))
                {
                    query = isNpgsql
                        ? query.Where(e => EF.Functions.ILike(e.ElementGroup.Category.Name, $"%{request.CategoryName}%"))
                        : query.Where(e => e.ElementGroup.Category.Name.ToLower().Contains(request.CategoryName.ToLower()));
                }



                var totalCount = await query.CountAsync();


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

                return new PagedResultDTO<ElementTreeDTO>(items, totalCount, request.PageNumber, request.PageSize);
            }
            throw new NotImplementedException("Empty request");
        }
    }
}

