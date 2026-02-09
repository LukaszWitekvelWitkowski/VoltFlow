using MediatR;
using VoltFlow.Service.Application.Commands.Category;
using VoltFlow.Service.Application.Services;
using VoltFlow.Service.Core.Models.Category.DTOs;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Infrastructure.Handlers.Category
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, ServiceResponse<CategoryDTO>>
    {
        private readonly ICategoryService _categoryService;

        public CreateCategoryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<ServiceResponse<CategoryDTO>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            return await _categoryService.CreateCategory(request.Name);
        }
    }
}
