using VoltFlow.Service.Core.Models.Category.DTOs;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Core.Abstractions.Repositories
{
    public interface ICategoryRepository
    {
        Task<ServiceResponse<CategoriesDTO>> GetCategoriesQuery();
        Task<ServiceResponse<CategoryDTO>> GetCategoryByIdQuery(int id);
        Task<ServiceResponse<PagedResultDTO<CategoryDTO>>> GetCategoriesPagedByNameQuery(string? name, int page, int size);
    }
}
