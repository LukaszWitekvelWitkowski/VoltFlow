using VoltFlow.Service.Core.Models;
using VoltFlow.Service.Core.Models.Category;

namespace VoltFlow.Service.Core.Abstractions.Repositories
{
    public interface ICategoryRepository
    {
        Task<ServiceResponse<CategoriesDTO>> GetCategoriesQuery();
        Task<ServiceResponse<CategoryDTO>> GetCategoryByIdQuery(int id);
        Task<ServiceResponse<CategoryDTO>> GetCategoryByNameQuery(string name);
    }
}
