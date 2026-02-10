using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Category.DTOs;
using VoltFlow.Service.Core.Models.Category.Request;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Test.Integration.IntegrationTest.Controllers
    {
        public class CategoryIntegrationTests : BaseIntegrationTest, IClassFixture<IntegrationTestFactory>
        {
            private readonly HttpClient _client;
            private readonly IntegrationTestFactory _factory;

            public CategoryIntegrationTests(IntegrationTestFactory factory) : base(factory)
            {
                _factory = factory;
                _client = factory.CreateClient();
            }

            #region GET Tests

            [Fact]
        public async Task GetCategories_ShouldReturnAllCategories()
        {
            // Arrange
            DbContext.categories.AddRange(new Category { Name = "Cat 1" }, new Category { Name = "Cat 2" });
            await DbContext.SaveChangesAsync();

            // Act
            var response = await Client.GetAsync("/api/category");
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<CategoriesDTO>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(2, result._Data.Categories.Count());
        }

        [Fact]
            public async Task GetCategoryById_ShouldReturn404_WhenCategoryDoesNotExist()
            {
                // Act
                var response = await _client.GetAsync("/api/category/9999");

                // Assert
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        [Fact]
        public async Task GetCategorySearch_ShouldReturnFilteredResults_WithPagination()
        {
            // 1. Arrange - Dodajemy dane bezpośrednio przez DbContext z klasy bazowej
            DbContext.categories.AddRange(
                new Category { Name = "Electronics" },
                new Category { Name = "Electric Tools" },
                new Category { Name = "Mechanical" }
            );
            await DbContext.SaveChangesAsync();

            // 2. Act
            var response = await Client.GetAsync("/api/category/search?name=electr&number=1&size=10");

            // 3. Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<PagedResultDTO<CategoryDTO>>>();

            Assert.NotNull(result?._Data);
            Assert.Equal(2, result._Data.TotalCount);
            Assert.Contains(result._Data.Results, x => x.Name == "Electronics");
        }

        #endregion

        #region POST Tests

        [Fact]
            public async Task CreateCategory_ShouldReturnCreated_AndSaveToPostgres()
            {
                // Arrange
                var request = new CreateCategoryRequest("New Integration Category");

                // Act
                var response = await _client.PostAsJsonAsync("/api/category", request);

                // Assert
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<CategoryDTO>>();
                Assert.Equal("New Integration Category", result._Data.Name);

                // Verify in DB
                using var scope = _factory.Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<VoltFlowDbContext>();
                Assert.True(context.categories.Any(c => c.Name == request.Name));
            }


        [Fact]
        public async Task CreateCategory_ShouldReturnConflict_WhenNameAlreadyExists()
        {
            // Arrange
            // Używamy DbContextu z klasy bazowej - bez EnsureDeleted!
            DbContext.categories.Add(new Category { Name = "Duplicate" });
            await DbContext.SaveChangesAsync();

            var request = new CreateCategoryRequest("Duplicate");

            // Act
            var response = await Client.PostAsJsonAsync("/api/category", request);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        #endregion

        #region PUT Tests

        [Fact]
        public async Task UpdateCategory_ShouldModifyExistingRecord_AndInvalidateCache()
        {
            // Arrange
            var category = new Category { Name = "Old Name" };

            // Używamy bezpośrednio DbContextu dostępnego w klasie bazowej
            DbContext.categories.Add(category);
            await DbContext.SaveChangesAsync();

            // Przygotowujemy żądanie (używamy ID, które nadała baza danych)
            var request = new UpdateCategoryRequest
            {
                Id = category.IdCategory,
                Name = "Updated Name",
                IsObsolete = false
            };

            // Act
            var response = await Client.PutAsJsonAsync("/api/category", request);

            // Assert
            response.EnsureSuccessStatusCode();

            // Sprawdzamy stan bazy po operacji (używając nowego scope lub odświeżając encję)
            // Najbezpieczniej pobrać świeże dane bezpośrednio z bazy
            var updated = await DbContext.categories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdCategory == category.IdCategory);

            Assert.NotNull(updated);
            Assert.Equal("Updated Name", updated.Name);
        }

        #endregion

        #region Helpers

        private async Task SeedDataAsync(params Category[] categories)
        {

            await DbContext.Database.EnsureDeletedAsync();
            await DbContext.Database.EnsureCreatedAsync();

            if (categories.Length != 0)
            {
                await DbContext.categories.AddRangeAsync(categories);
                await DbContext.SaveChangesAsync();
            }

            if (_scope.ServiceProvider.GetService<IMemoryCache>() is MemoryCache concreteCache)
            {
                concreteCache.Compact(1.0); 
            }
        }

        #endregion
    }
}
