using VoltFlow.Service.Core.Models.Category.DTOs;
using VoltFlow.Service.Core.Models.Category.Request;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Core.Abstractions.Repositories
{
    public interface ICategoryRepository
    {
        Task<CategoryCacheDTO> GetCategoriesQuery();
        Task<CategoryDTO> GetCategoryByIdQuery(int id);
        Task<PagedResultDTO<CategoryDTO>> GetCategoriesPagedByNameQuery(string? name, int page, int size);
        Task<CategoryDTO> CategoryExistsByName(string name);
        Task<CategoryDTO> AddCategory(string categoryDto);
        Task<CategoryDTO> UpdateCategory(UpdateCategoryRequest request);
        Task<bool> IsExists(string name, int? id = null);
    }
}
