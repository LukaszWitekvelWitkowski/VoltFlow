using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models.Category.DTOs;
using VoltFlow.Service.Core.Models.Category.Request;
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
        public async Task<ServiceResponse<CategoryDTO>> CreateCategory(string name)
        {
            var validation = ValidateName(name);
            if (!validation._IsSuccess) return validation;

            var duplicateCheck = await CheckForDuplicateName(name);
            if (!duplicateCheck._IsSuccess) return duplicateCheck;

            return await _categoryRepository.AddCategory(name);
        }

        public async Task<ServiceResponse<CategoryDTO>> UpdateCategory(UpdateCategoryRequest request)
        {
            var validation = ValidateName(request.Name);
            if (!validation._IsSuccess) return validation;

            var allCategories = await _categoryRepository.GetCategoriesQuery();
            var currentCategory = allCategories._Data?.Categories.FirstOrDefault(c => c.Id == request.Id);

            if (currentCategory == null)
                return ServiceResponse<CategoryDTO>.Failure("Kategoria nie istnieje.", 404);

            if (IsDataUnchanged(currentCategory, request))
            {
                return ServiceResponse<CategoryDTO>.Failure("Nie wprowadzono żadnych zmian.", 200);
            }

            var duplicateCheck = await CheckForDuplicateName(request.Name, request.Id);
            if (!duplicateCheck._IsSuccess) return duplicateCheck;

            return await _categoryRepository.UpdateCategory(request);
        }

        #region Private Helper Methods

        private ServiceResponse<CategoryDTO> ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return ServiceResponse<CategoryDTO>.Failure("Nazwa kategorii nie może być pusta.", 400);
            }
            return ServiceResponse<CategoryDTO>.Result(null!);
        }

        private async Task<ServiceResponse<CategoryDTO>> CheckForDuplicateName(string name, int? excludeId = null)
        {
            var allCategories = await _categoryRepository.GetCategoriesQuery();

            var exists = allCategories._Data?.Categories
                .Any(c => c.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase)
                          && (excludeId == null || c.Id != excludeId));

            if (exists == true)
            {
                return ServiceResponse<CategoryDTO>.Failure("Kategoria o podanej nazwie już istnieje w systemie.", 409);
            }

            return ServiceResponse<CategoryDTO>.Result(null!);
        }

        private bool IsDataUnchanged(CategoryDTO current, UpdateCategoryRequest request)
        {
            return current.Name.Equals(request.Name.Trim(), StringComparison.OrdinalIgnoreCase)
                   && current.IsObsolete == request.IsObsolete;
        }

        #endregion
    }
}
