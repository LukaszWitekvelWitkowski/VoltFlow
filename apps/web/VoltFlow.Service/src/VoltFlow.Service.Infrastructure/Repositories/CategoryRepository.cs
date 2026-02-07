using Microsoft.EntityFrameworkCore;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Category.DTOs;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Pagination;
using VoltFlow.Service.Core.Tools;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly VoltFlowDbContext _context;
        private readonly LazyValue<ServiceResponse<CategoriesDTO>> _allCategoryLazy;

        public CategoryRepository(VoltFlowDbContext context)
        {
            _context = context;
            // Inicjalizacja leniwego ładowania z delegatem do metody pobierającej
            _allCategoryLazy = new LazyValue<ServiceResponse<CategoriesDTO>>(FetchCategoriesFromDb);
        }

        public async Task<ServiceResponse<CategoriesDTO>> GetCategoriesQuery()
        {
            var response = await _allCategoryLazy.GetValueAsync();
            // Zwracamy surowe dane (kontener CategoriesDTO)
            return ServiceResponse<CategoriesDTO>.Result(response._Data);
        }

        public async Task<ServiceResponse<CategoryDTO>> GetCategoryByIdQuery(int id)
        {
            var response = await _allCategoryLazy.GetValueAsync();

            if (!response._IsSuccess)
            {
                return ServiceResponse<CategoryDTO>.Failure(response._Message, response._StatusCode);
            }

            // Szukamy w pamięci RAM
            var category = response._Data?.Categories.FirstOrDefault(c => c.Id == id);

            return ServiceResponse<CategoryDTO>.Result(category);
        }

        public async Task<ServiceResponse<PagedResultDTO<CategoryDTO>>> GetCategoriesPagedByNameQuery(string? name, int page, int size)
        {
            var response = await _allCategoryLazy.GetValueAsync();

            if (!response._IsSuccess)
            {
                return ServiceResponse<PagedResultDTO<CategoryDTO>>.Failure(response._Message, response._StatusCode);
            }

            // Przygotowanie listy pod generycznego Helpera
            var listResponse = ServiceResponse<List<CategoryDTO>>.Result(response._Data?.Categories ?? new List<CategoryDTO>());

            return PagedHelper.ToPagedResponse(
                listResponse,
                name,
                c => c.Name,
                page,
                size
            );
        }

        public async Task<ServiceResponse<CategoryDTO>> GetCategoryByNameQuery(string name)
        {
            var response = await _allCategoryLazy.GetValueAsync();

            if (!response._IsSuccess)
            {
                return ServiceResponse<CategoryDTO>.Failure(response._Message, response._StatusCode);
            }

            var category = response._Data?.Categories
                .FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            return ServiceResponse<CategoryDTO>.Result(category);
        }

        private async Task<ServiceResponse<CategoriesDTO>> FetchCategoriesFromDb()
        {
            try
            {
                var categoriesList = await _context.Set<Category>()
                    .AsNoTracking()
                    .Select(c => new CategoryDTO
                    {
                        Id = c.IdCategory,
                        Name = c.Name,
                        IsObsolete = c.IsObsolete
                    }).ToListAsync();

                return ServiceResponse<CategoriesDTO>.Result(new CategoriesDTO { Categories = categoriesList });
            }
            catch (Exception)
            {
                // Tutaj warto dodać logowanie ex
                return ServiceResponse<CategoriesDTO>.Failure("Błąd podczas pobierania kategorii z bazy.", 500);
            }
        }
    }
}