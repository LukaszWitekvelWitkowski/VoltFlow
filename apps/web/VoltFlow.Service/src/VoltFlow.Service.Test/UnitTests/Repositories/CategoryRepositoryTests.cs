using Microsoft.EntityFrameworkCore;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Exceptions;
using VoltFlow.Service.Core.Models.Category.DTOs;
using VoltFlow.Service.Core.Models.Category.Request;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;
using VoltFlow.Service.Infrastructure.Repositories;

namespace VoltFlow.Service.Test.UnitTests.Repositories
{
    public class CategoryRepositoryTests : VoltFlowTestBase
    {
        private readonly CategoryRepository _repository;

        public CategoryRepositoryTests() : base()
        {
            _repository = new CategoryRepository(_context, _configuration);
        }
       
        [Fact]
        public async Task GetCategoryByIdQuery_ShouldReturnCorrectData_FromDatabase()
        {
            CacheRepository<CategoryCacheDTO, CategoryDTO, Category>.ResetStaticCache();
            // Arrange
            var category = new Category { IdCategory = 1, Name = "Electronics", IsObsolete = false };
            _context.Set<Category>().Add(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetCategoryByIdQuery(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Electronics", result.Name);
        }

        [Fact]
        public async Task AddCategory_ShouldPersistInDb_AndInvalidateCache()
        {
            // Arrange
            string newName = "New Category";

            // Act
            var addResult = await _repository.AddCategory(newName);
            var getResult = await _repository.GetCategoriesQuery();

            // Assert
            Assert.Contains(getResult.Items, c => c.Name == newName);
            Assert.Equal(1, await _context.Set<Category>().CountAsync());
        }

        [Fact]
        public async Task UpdateCategory_ShouldUpdateData_AndThrowNotFoundIfMissing()
        {
            // Arrange
            var category = new Category { IdCategory = 10, Name = "Old Name" };
            _context.Set<Category>().Add(category);
            await _context.SaveChangesAsync();

            var request = new UpdateCategoryRequest { Id = 10, Name = "Updated Name", IsObsolete = true };

            // Act
            var result = await _repository.UpdateCategory(request);

            // Assert
            Assert.Equal("Updated Name", result.Name);
            Assert.True(result.IsObsolete);

            // Test NotFound
            var badRequest = new UpdateCategoryRequest { Id = 99, Name = "None" };
            await Assert.ThrowsAsync<NotFoundException>(() => _repository.UpdateCategory(badRequest));
        }

        [Fact]
        public async Task IsExists_ShouldBeCaseInsensitive_AndHandleCache()
        {
            // Arrange
            _context.Set<Category>().Add(new Category { IdCategory = 1, Name = "Tools" });
            await _context.SaveChangesAsync();

            // Act
            var exists = await _repository.IsExists("tools"); // lower case
            var existsWithTrim = await _repository.IsExists("  TOOLS  "); // spaces and capital letters

            // Assert
            Assert.True(exists);
            Assert.True(existsWithTrim);
        }

        [Fact]
        public async Task GetCategoriesPagedByNameQuery_ShouldReturnFilteredResults()
        {
            CacheRepository<CategoryCacheDTO, CategoryDTO, Category>.ResetStaticCache();
            // Arrange
            _context.Set<Category>().AddRange(new List<Category>
            {
                new Category { IdCategory = 1, Name = "Spare parts" },
                new Category { IdCategory = 2, Name = "Electronic components" },
                new Category { IdCategory = 3, Name = "Electrical components" }
 
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetCategoriesPagedByNameQuery("a", 1, 10);

            // Assert
            Assert.Equal(2, result.TotalCount);
            Assert.Contains(result.Results, c => c.Name == "Spare parts");
            Assert.Contains(result.Results, c => c.Name == "Electrical components");
        }

        [Fact]
        public async Task CacheThreshold_ShouldDisableCache_WhenDataTooLarge()
        {
            // Arrange
            for (int i = 0; i < 101; i++)
            {
                _context.Set<Category>().Add(new Category { IdCategory = i + 1, Name = $"Cat {i}" });
            }
            await _context.SaveChangesAsync();

            // Act
            await _repository.GetCategoriesQuery();

            // Assert
            var countInDb = await _context.Set<Category>().CountAsync();
            Assert.Equal(101, countInDb);
        }

    }
}