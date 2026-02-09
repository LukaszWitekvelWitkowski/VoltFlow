using VoltFlow.Service.Core.Models.Category.DTOs;
using VoltFlow.Service.Core.Models.Category.Request;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Core.Abstractions.Repositories
{
    public interface ICategoryRepository
    {
        Task<ServiceResponse<CategoriesDTO>> GetCategoriesQuery();
        Task<ServiceResponse<CategoryDTO>> GetCategoryByIdQuery(int id);
        Task<ServiceResponse<PagedResultDTO<CategoryDTO>>> GetCategoriesPagedByNameQuery(string? name, int page, int size);
        Task<CategoryDTO> CategoryExistsByName(string name);
        Task<ServiceResponse<CategoryDTO>> AddCategory(string categoryDto);
        Task<ServiceResponse<CategoryDTO>> UpdateCategory(UpdateCategoryRequest request);
        Task<bool> IsExists(string name, int? id = null);
    }
}
