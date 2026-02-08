using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models.Category.DTOs;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<ServiceResponse<CategoryDTO>> CreateCategoryCommand(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return ServiceResponse<CategoryDTO>.Failure("Nazwa kategorii nie może być pusta.", 400);
            }

            bool exists = await _categoryRepository.CategoryExistsByName(name);

            if (exists)
            {
                return ServiceResponse<CategoryDTO>.Failure("Kategoria o podanej nazwie już istnieje w systemie.", 409);
            }

            // 2. Jeśli nie istnieje, zlecamy zapis
            return await _categoryRepository.AddCategoryCommand(name);
        }
    }
}
