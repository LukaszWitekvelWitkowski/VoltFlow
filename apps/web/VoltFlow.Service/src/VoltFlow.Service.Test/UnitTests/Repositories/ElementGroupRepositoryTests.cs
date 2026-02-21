using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Exceptions;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;
using VoltFlow.Service.Core.Models.ElementGroup.Request;
using VoltFlow.Service.Infrastructure.Repositories;

namespace VoltFlow.Service.Test.UnitTests.Repositories
{
    public class ElementGroupRepositoryTests : VoltFlowTestBase
    {
        private readonly ElementGroupRepository _repository;

        public ElementGroupRepositoryTests() : base()
        {
            _repository = new ElementGroupRepository(_context, _configuration);
        }

        [Fact]
        public async Task AddElementGroup_ShouldThrowNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var request = new CreateElementGroupRequest("New Group", 999);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _repository.AddElementGroup(request));
        }

        [Fact]
        public async Task AddElementGroup_ShouldSucceed_AndInvalidateCache()
        {
            CacheRepository<ElementGroupCacheDTO, ElementGroupDTO, ElementGroup>.ResetStaticCache();
            // Arrange
            var category = new Category { IdCategory = 1, Name = "Main Category" };
            await SeedDataAsync(new[] { category });

            var request = new CreateElementGroupRequest("Valid Group", 1);

            // Act
            await _repository.GetElementGroupsQuery();

            var result = await _repository.AddElementGroup(request);

            var finalQuery = await _repository.GetElementGroupsQuery();

            // Assert
            Assert.Contains(finalQuery.Items, eg => eg.Name == "Valid Group");
        }

        [Fact]
        public async Task GetElementGroupSearchQuery_ShouldReturnPagedResults_FromCache()
        {
            CacheRepository<ElementGroupCacheDTO, ElementGroupDTO, ElementGroup>.ResetStaticCache();
            // Arrange
            var category = new Category { IdCategory = 1, Name = "Cat" };
            var groups = new List<ElementGroup>
            {
                new ElementGroup { IdElementGroup = 1, Name = "Power Supply", CategoryId = 1 },
                new ElementGroup { IdElementGroup = 2, Name = "Cables", CategoryId = 1 },
                new ElementGroup { IdElementGroup = 3, Name = "Connectors", CategoryId = 1 }
            };
            await SeedDataAsync(new[] { category });
            await SeedDataAsync(groups);

            // Act
            var result = await _repository.GetElementGroupSearchQuery("co", 1, 10);

            // Assert
            Assert.Single(result.Results);
            Assert.Equal("Connectors", result.Results.First().Name);
            Assert.Equal(1, result.TotalCount);
        }

        [Fact]
        public async Task UpdateElementGroup_ShouldChangeData_AndValidateCategory()
        {
            // Arrange
            await SeedDataAsync(new[] { new Category { IdCategory = 1, Name = "C1" } });
            await SeedDataAsync(new[] { new ElementGroup { IdElementGroup = 5, Name = "Old Name", CategoryId = 1 } });

            var request = new UpdateElementGroupRequest
            {
                IdElementGroup = 5,
                Name = "New Name",
                CategoryId = 1,
                IsObsolete = true
            };

            // Act
            var result = await _repository.UpdateElementGroup(request);

            // Assert
            Assert.Equal("New Name", result.Name);
            Assert.True(result.IsObsolete);
        }

        [Fact]
        public async Task IsExists_ShouldIgnoreCurrentId_DuringUpdateValidation()
        {
            CacheRepository<ElementGroupCacheDTO, ElementGroupDTO, ElementGroup>.ResetStaticCache();
            // Arrange
            await SeedDataAsync(new[] { new Category { IdCategory = 1, Name = "C1" } });
            var existing = new ElementGroup { IdElementGroup = 10, Name = "UniqueGroup", CategoryId = 1 };
            await SeedDataAsync(new[] { existing });

            // Act
            var existsSameId = await _repository.IsExists("UniqueGroup", 10);
            var existsDifferentId = await _repository.IsExists("UniqueGroup", 20);

            // Assert
            Assert.False(existsSameId);
            Assert.True(existsDifferentId); 
        }

    [Fact]
        public async Task UpdateElementGroup_ShouldThrowNotFound_WhenGroupDoesNotExist()
        {
            // Arrange
            var request = new UpdateElementGroupRequest
            {
                IdElementGroup = 999, 
                Name = "Updated Name",
                CategoryId = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _repository.UpdateElementGroup(request));
        }

        [Fact]
        public async Task UpdateElementGroup_ShouldThrowNotFound_WhenTargetCategoryDoesNotExist()
        {
            // Arrange
            var category = new Category { IdCategory = 1, Name = "Existing Cat" };
            var group = new ElementGroup { IdElementGroup = 10, Name = "Group", CategoryId = 1 };
            await SeedDataAsync(new[] { category });
            await SeedDataAsync(new[] { group });

            var request = new UpdateElementGroupRequest
            {
                IdElementGroup = 10,
                Name = "New Name",
                CategoryId = 888 
            };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _repository.UpdateElementGroup(request));
        }

        [Fact]
        public async Task AddElementGroup_ShouldHandleDuplicateNames_ViaIsExistsValidation()
        {
            CacheRepository<ElementGroupCacheDTO, ElementGroupDTO, ElementGroup>.ResetStaticCache();
            // Arrange
            var category = new Category { IdCategory = 1, Name = "Cat" };
            var group = new ElementGroup { IdElementGroup = 1, Name = "UniqueName", CategoryId = 1 };
            await SeedDataAsync(new[] { category });
            await SeedDataAsync(new[] { group });

            // Act
            var existsLower = await _repository.IsExists("uniquename"); // Case insensitive
            var existsUpper = await _repository.IsExists("UNIQUENAME");
            var existsWithSpaces = await _repository.IsExists("  UniqueName  ");

            // Assert
            Assert.True(existsLower);
            Assert.True(existsUpper);
            Assert.True(existsWithSpaces);
        }

        [Fact]
        public async Task GetElementGroupSearchQuery_ShouldReturnEmpty_WhenNoMatchFound()
        {
            CacheRepository<ElementGroupCacheDTO, ElementGroupDTO, ElementGroup>.ResetStaticCache();
            // Arrange
            var category = new Category { IdCategory = 1, Name = "Cat" };
            await SeedDataAsync(new[] { category });
            await SeedDataAsync(new[] { new ElementGroup { Name = "Resistors", CategoryId = 1 } });

            // Act
            var result = await _repository.GetElementGroupSearchQuery("NonExistentString", 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Results);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task GetElementGroupByIdQuery_ShouldReturnNullData_WhenIdIsInvalid()
        {
            // Arrange
            CacheRepository<ElementGroupCacheDTO, ElementGroupDTO, ElementGroup>.ResetStaticCache();

            // Act
            var result = await _repository.GetElementGroupByIdQuery(0);

            // Assert
            Assert.Null(result);
        }
    }
}
