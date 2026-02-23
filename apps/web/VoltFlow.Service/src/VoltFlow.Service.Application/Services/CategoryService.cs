using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Exceptions;
using VoltFlow.Service.Core.Models.Category.DTOs;
using VoltFlow.Service.Core.Models.Category.Request;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository) => _categoryRepository = categoryRepository;

        public async Task<ServiceResponse<CategoryDTO>> CreateCategory(string name)
        {
            // 1. Validation
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationEntityException("Nazwa kategorii nie może być pusta.");

            // 2. Optimal duplicate checking (without downloading the entire list)
            if (await _categoryRepository.IsExists(name))
                throw new ConflictException("Kategoria o podanej nazwie już istnieje w systemie.");

            // 3. Save
            var result = await _categoryRepository.AddCategory(name);
            return ServiceResponse<CategoryDTO>.Success(result);
        }

        public async Task<ServiceResponse<CategoryDTO>> UpdateCategory(UpdateCategoryRequest request)
        {
            // 1. Name validation
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ValidationEntityException("Nazwa kategorii nie może być pusta.");

            // 2. Get the current state (The repository should throw an error or return null)
            var currentCategory = (await _categoryRepository.GetCategoryByIdQuery(request.Id))
                                  ?? throw new NotFoundException("Kategoria nie istnieje.");

            // 3. Checking if data has changed (Idempotence)
            if (IsDataUnchanged(currentCategory, request))

                return ServiceResponse<CategoryDTO>.Success(currentCategory);

            // 4. Checking for duplicates when renaming
            if (await _categoryRepository.IsExists(request.Name, request.Id))
                throw new ConflictException("Kategoria o podanej nazwie już istnieje w systemie.");

            // 5. Update
            var updated = await _categoryRepository.UpdateCategory(request);
            return ServiceResponse<CategoryDTO>.Success(updated);
        }

        #region Private Helper Methods

        private bool IsDataUnchanged(CategoryDTO current, UpdateCategoryRequest request)
        {
            return current.Name.Trim().Equals(request.Name.Trim(), StringComparison.OrdinalIgnoreCase)
                   && current.IsObsolete == request.IsObsolete;
        }

        #endregion
    }
}
