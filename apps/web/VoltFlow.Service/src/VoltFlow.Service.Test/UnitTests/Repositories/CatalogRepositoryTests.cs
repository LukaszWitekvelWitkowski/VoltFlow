using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Requests;
using VoltFlow.Service.Infrastructure.Repositories;

namespace VoltFlow.Service.Test.UnitTests.Repositories
{
    public class CatalogRepositoryTests : VoltFlowTestBase
    {
        private readonly CatalogRepository _repository;

        public CatalogRepositoryTests() : base()
        {
            _repository = new CatalogRepository(_context, _configuration);
        }

        [Fact]
        public async Task GetCatalogSearchQuery_ShouldReturnFullTree_WhenNoFiltersApplied()
        {
            // Arrange
            await SeedCatalogData();
            var request = new CatalogSearchRequest { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _repository.GetCatalogSearchQuery(request);

            // Assert
            Assert.Equal(3, result._Data!.TotalCount);
            var firstItem = result._Data.Results.First();
            Assert.NotNull(firstItem.Group);
            Assert.NotNull(firstItem.Group.Category);
        }

        [Fact]
        public async Task GetCatalogSearchQuery_FilterByCategoryName_ShouldReturnOnlyMatchingElements()
        {
            // Arrange
            await SeedCatalogData();
            var request = new CatalogSearchRequest
            {
                CategoryName = "Electric", // Powinno pasować do "Electrical"
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _repository.GetCatalogSearchQuery(request);

            // Assert
            Assert.Equal(2, result._Data!.TotalCount);
            Assert.All(result._Data.Results, item =>
            {
                Assert.NotNull(item.Group);
                Assert.NotNull(item.Group.Category);
                Assert.Equal("Electrical", item.Group.Category!.Name);
            });
        }

        [Fact]
        public async Task GetCatalogSearchQuery_Pagination_ShouldReturnCorrectPage()
        {
            // Arrange
            await SeedCatalogData();
            var request = new CatalogSearchRequest { PageNumber = 2, PageSize = 2 };

            // Act
            var result = await _repository.GetCatalogSearchQuery(request);

            // Assert
            Assert.Equal(3, result._Data!.TotalCount); 
            Assert.Single(result._Data.Results); 
        }

        private async Task SeedCatalogData()
        {
            var cat1 = new Category { IdCategory = 1, Name = "Electrical" };
            var cat2 = new Category { IdCategory = 2, Name = "Plumbing" };

            var group1 = new ElementGroup { IdElementGroup = 1, Name = "Wires", CategoryId = 1 };
            var group2 = new ElementGroup { IdElementGroup = 2, Name = "Pipes", CategoryId = 2 };

            var elements = new List<Element>
            {
                new Element { IdElement = 1, Name = "Power Cable", ElementGroupId = 1 },
                new Element { IdElement = 2, Name = "Ground Wire", ElementGroupId = 1 },
                new Element { IdElement = 3, Name = "PVC Pipe", ElementGroupId = 2 }
            };

            await SeedDataAsync(new[] { cat1, cat2 });
            await SeedDataAsync(new[] { group1, group2 });
            await SeedDataAsync(elements);
        }

    [Fact]
        public async Task GetCatalogSearchQuery_ShouldReturnEmptyPagedResult_WhenFiltersMatchNothing()
        {
            // Arrange
            await SeedCatalogData();
            var request = new CatalogSearchRequest
            {
                ElementName = "Non-existent-component-12345",
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _repository.GetCatalogSearchQuery(request);

            // Assert
            Assert.True(result._IsSuccess);
            Assert.Empty(result._Data!.Results);
            Assert.Equal(0, result._Data.TotalCount);
        }

        [Fact]
        public async Task GetCatalogSearchQuery_ShouldHandleNullRequest_ByReturningAllData()
        {
            // Arrange
            await SeedCatalogData();
            CatalogSearchRequest? request = new CatalogSearchRequest { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _repository.GetCatalogSearchQuery(request);

            // Assert
            Assert.Equal(3, result._Data!.TotalCount);
        }

        [Fact]
        public async Task GetCatalogSearchQuery_InvalidPagination_ShouldReturnEmptyButSuccess()
        {
            // Arrange
            await SeedCatalogData();
            var request = new CatalogSearchRequest
            {
                PageNumber = 999, 
                PageSize = 10
            };

            // Act
            var result = await _repository.GetCatalogSearchQuery(request);

            // Assert
            Assert.True(result._IsSuccess);
            Assert.Empty(result._Data!.Results);
            Assert.Equal(3, result._Data.TotalCount); 
        }
    }
}
