using MediatR;
using VoltFlow.Service.Application.Queries.Category;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models;
using VoltFlow.Service.Core.Models.Category;

namespace VoltFlow.Service.Infrastructure.Handlers.Category
{
    public  class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery,ServiceResponse<CategoryDTO>>
    {
        private readonly ICategoryRepository _categoryRepository;
        public GetCategoryByIdHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<ServiceResponse<CategoryDTO>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            return ResponseValidator.EnsureSuccessAndData(await _categoryRepository.GetCategoryByIdQuery(request.Id), "Categories");
        }
    }
}
