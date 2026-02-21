using MediatR;
using VoltFlow.Service.Application.Queries.Category;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models.Category.DTOs;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Validators;

namespace VoltFlow.Service.Infrastructure.Handlers.Category
{
    public class GetCategoriesHandlers : IRequestHandler<GetCategoriesQuery, ServiceResponse<CategoryCacheDTO>>
    {
        private readonly ICategoryRepository _categoryRepository;
        public GetCategoriesHandlers(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<ServiceResponse<CategoryCacheDTO>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await ResponseValidator.ExecuteAsync(async () => await _categoryRepository.GetCategoriesQuery(), "Categories");
        }
    }
}
