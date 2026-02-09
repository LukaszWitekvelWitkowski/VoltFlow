using VoltFlow.Service.Core.Models.Category.DTOs;
using VoltFlow.Service.Core.Models.Category.Request;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Services
{
    public interface ICategoryService
    {
        Task<ServiceResponse<CategoryDTO>> CreateCategory(string name);

        Task<ServiceResponse<CategoryDTO>> UpdateCategory(UpdateCategoryRequest request);
    }
}
