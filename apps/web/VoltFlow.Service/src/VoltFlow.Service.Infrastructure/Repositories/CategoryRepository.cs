using Microsoft.EntityFrameworkCore;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models;
using VoltFlow.Service.Core.Models.Category;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly VoltFlowDbContext _context;

        public CategoryRepository(VoltFlowDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<CategoriesDTO>> GetCategoriesQuery()
        {
            var categoriesList = await _context.Set<Category>()
                    .AsNoTracking()
                    .Select(c => new CategoryDTO
                    {
                        Id = c.IdCategory,
                        Name = c.Name,
                        IsObsolete = c.IsObsolete
                    })
                    .ToListAsync();

            var data = new CategoriesDTO
            {
                Categories = categoriesList
            };

            return ServiceResponse<CategoriesDTO>.Result(data);
        }

        public async Task<ServiceResponse<CategoryDTO>> GetCategoryByIdQuery(int id)
        {
            var category = await _context.Set<Category>()
                    .AsNoTracking()
                    .Where(c => c.IdCategory == id)
                    .Select(c => new CategoryDTO
                    {
                        Id = c.IdCategory,
                        Name = c.Name,
                        IsObsolete = c.IsObsolete
                    })
                    .FirstOrDefaultAsync();

            return ServiceResponse<CategoryDTO>.Result(category);
        }

        public async Task<ServiceResponse<CategoryDTO>> GetCategoryByNameQuery(string name)
        {
            var category = await _context.Set<Category>()
                    .AsNoTracking()
                    .Where(c => c.Name == name)
                    .Select(c => new CategoryDTO
                    {
                        Id = c.IdCategory,
                        Name = c.Name,
                        IsObsolete = c.IsObsolete
                    })
                    .FirstOrDefaultAsync();

            return ServiceResponse<CategoryDTO>.Result(category);
        }
    }
}
