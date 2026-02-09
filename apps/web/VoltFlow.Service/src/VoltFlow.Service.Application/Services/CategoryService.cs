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
            // 1. Walidacja
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationEntityException("Nazwa kategorii nie może być pusta.");

            // 2. Optymalne sprawdzenie duplikatu (bez pobierania całej listy)
            if (await _categoryRepository.IsExists(name))
                throw new ConflictException("Kategoria o podanej nazwie już istnieje w systemie.");

            // 3. Zapis
            var result = await _categoryRepository.AddCategory(name);
            return ServiceResponse<CategoryDTO>.Success(result._Data!);
        }

        public async Task<ServiceResponse<CategoryDTO>> UpdateCategory(UpdateCategoryRequest request)
        {
            // 1. Walidacja nazwy
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ValidationEntityException("Nazwa kategorii nie może być pusta.");

            // 2. Pobranie aktualnego stanu (Repozytorium powinno rzucać błąd lub zwracać null)
            var currentCategory = (await _categoryRepository.GetCategoryByIdQuery(request.Id))._Data
                                  ?? throw new NotFoundException("Kategoria nie istnieje.");

            // 3. Sprawdzenie czy dane się zmieniły (Idempotentność)
            if (IsDataUnchanged(currentCategory, request))
                return ServiceResponse<CategoryDTO>.Success(currentCategory);

            // 4. Sprawdzenie duplikatu przy zmianie nazwy
            if (await _categoryRepository.IsExists(request.Name, request.Id))
                throw new ConflictException("Kategoria o podanej nazwie już istnieje w systemie.");

            // 5. Aktualizacja
            var updated = await _categoryRepository.UpdateCategory(request);
            return ServiceResponse<CategoryDTO>.Success(updated._Data!);
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
