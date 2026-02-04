using MediatR;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models;
using VoltFlow.Service.Core.Models.Category;

namespace VoltFlow.Service.Infrastructure.Handlers.Category
{
    public class GetCategoryByNameHandler : IRequestHandler<GetCategoryByNameQuery, ServiceResponse<CategoryDTO>>
    {
        private readonly ICategoryRepository _categoryRepository;
        public GetCategoryByNameHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<ServiceResponse<CategoryDTO>> Handle(GetCategoryByNameQuery request, CancellationToken cancellationToken)
        {
            return ResponseValidator.EnsureSuccessAndData(await _categoryRepository.GetCategoryByNameQuery(request.Name), "Category");
        }
    }
}
