using MediatR;
using VoltFlow.Service.Application.Queries.Category;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models;
using VoltFlow.Service.Core.Models.Category;

namespace VoltFlow.Service.Infrastructure.Handlers.Category
{
    public class GetCategoriesHandlers : IRequestHandler<GetCategoriesQuery, ServiceResponse<CategoriesDTO>>
    {
        private readonly ICategoryRepository _categoryRepository;
        public GetCategoriesHandlers(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<ServiceResponse<CategoriesDTO>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            return ResponseValidator.EnsureSuccessAndData(await _categoryRepository.GetCategoriesQuery(), "Categories");
        }
    }
}
