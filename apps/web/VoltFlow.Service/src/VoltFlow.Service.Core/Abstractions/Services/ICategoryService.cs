using VoltFlow.Service.Core.Models.Category.DTOs;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Services
{
    public interface ICategoryService
    {
        public Task<ServiceResponse<CategoryDTO>> CreateCategoryCommand(string name);
    }
}
