using MediatR;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models.Category;
using VoltFlow.Service.Core.Models.Category.DTOs;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Validators;

namespace VoltFlow.Service.Infrastructure.Handlers.Category
{
    public class GetCategoryByNameHandler : IRequestHandler<GetCategoryByNameQuery, ServiceResponse<PagedResultDTO<CategoryDTO>>>
    {
        private readonly ICategoryRepository _categoryRepository;
        public GetCategoryByNameHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<ServiceResponse<PagedResultDTO<CategoryDTO>>> Handle(GetCategoryByNameQuery request, CancellationToken cancellationToken)
        {
            return ResponseValidator.EnsureSuccessAndData(await _categoryRepository.GetCategoriesPagedByNameQuery(request.Name, request.PageNumber, request.PageSize), "Category");
        }
    }
}
