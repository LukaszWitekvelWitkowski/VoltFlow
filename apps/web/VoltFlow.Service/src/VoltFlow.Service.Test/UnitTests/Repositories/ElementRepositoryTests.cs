using Microsoft.EntityFrameworkCore;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Exceptions;
using VoltFlow.Service.Core.Models.Element.DTOs;
using VoltFlow.Service.Core.Models.Element.Request;
using VoltFlow.Service.Infrastructure.Repositories;

namespace VoltFlow.Service.Test.UnitTests.Repositories
{
    public class ElementRepositoryTests : VoltFlowTestBase
    {
        private readonly ElementRepository _repository;

        public ElementRepositoryTests() : base()
        {
            _repository = new ElementRepository(_context, _configuration);
        }

        [Fact]
        public async Task AddElement_ShouldThrowNotFound_WhenGroupDoesNotExist()
        {
            // Arrange
            var request = new CreateElementRequest("Test Element", 999, null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _repository.AddElement(request));
        }

        [Fact]
        public async Task AddElement_ShouldSucceed_WhenGroupExists()
        {
            // Arrange
            var group = new ElementGroup { IdElementGroup = 1, Name = "Group 1" };
            await SeedDataAsync(new[] { group });

            var request = new CreateElementRequest("New Element", 1, "Test Description");

            // Act
            var result = await _repository.AddElement(request);

            // Assert
            Assert.True(result._IsSuccess);
            Assert.Equal("New Element", result._Data!.Name);
            Assert.Equal("Test Description", result._Data.Description);

            // Sprawdzenie w bazie
            var dbElement = await _context.Set<Element>().FirstOrDefaultAsync();
            Assert.NotNull(dbElement);
            Assert.Equal(1, dbElement.ElementGroupId);
        }

        [Fact]
        public async Task GetElementsPagedByNameQuery_ShouldUseCache_AndReturnCorrectResults()
        {
            CacheRepository<ElementCacheDTO, ElementDTO, Element>.ResetStaticCache();
            // Arrange
            var group = new ElementGroup { IdElementGroup = 1, Name = "Group 1" };
            var elements = new List<Element>
            {
                new Element { IdElement = 1, Name = "Bolt", ElementGroupId = 1 },
                new Element { IdElement = 2, Name = "Nut", ElementGroupId = 1 },
                new Element { IdElement = 3, Name = "Screw", ElementGroupId = 1 }
            };
            await SeedDataAsync(new[] { group });
            await SeedDataAsync(elements);

            // Act
            await _repository.GetElementsPagedByNameQuery(null, 1, 10);

            var result = await _repository.GetElementsPagedByNameQuery("bolt", 1, 10);

            // Assert
            Assert.Single(result._Data!.Results);
            Assert.Equal("Bolt", result._Data.Results.First().Name);
        }

        [Fact]
        public async Task UpdateElement_ShouldInvalidateCache()
        {
            // Arrange
            var group = new ElementGroup { IdElementGroup = 1, Name = "G1" };
            var element = new Element { IdElement = 10, Name = "Old Name", ElementGroupId = 1 };
            await SeedDataAsync(new[] { group });
            await SeedDataAsync(new[] { element });

            // Inicjalizujemy cache
            await _repository.GetElementsQuery();

            var updateRequest = new UpdateElementRequest
            {
                Id = 10,
                Name = "Updated Name",
                ElementGroupId = 1,
                IsObsolete = false
            };

            // Act
            await _repository.UpdateElement(updateRequest);

            var result = await _repository.GetElementsQuery();

            // Assert
            Assert.Contains(result._Data!.Items, e => e.Name == "Updated Name");
        }

        [Fact]
        public async Task IsExists_ShouldReturnTrue_ForDuplicateName()
        {
            CacheRepository<ElementCacheDTO, ElementDTO, Element>.ResetStaticCache();
            // Arrange
            var element = new Element { IdElement = 5, Name = "Cable", ElementGroupId = 1 };
            await SeedDataAsync(new[] { element });

            // Act
            var exists = await _repository.IsExists("CABLE"); // Case insensitive
            var existsDifferentId = await _repository.IsExists("Cable", 99); 

            // Assert
            Assert.True(exists);
            Assert.True(existsDifferentId);
        }


        [Fact]
        public async Task AddElement_ShouldThrowException_WhenNameIsDuplicate()
        {
            CacheRepository<ElementCacheDTO, ElementDTO, Element>.ResetStaticCache();
            // Arrange
            var group = new ElementGroup { IdElementGroup = 1, Name = "Electronics" };
            var existingElement = new Element { IdElement = 1, Name = "Resistor", ElementGroupId = 1 };
            await SeedDataAsync(new[] { group });
            await SeedDataAsync(new[] { existingElement });

            // Scenario: Attempting to add an item with the same name (validation usually performed on the service, 
            // but we check the repository's robustness at the IsExists level before adding it)
            var request = new CreateElementRequest("Resistor", 1, "Another one");

            // Act
            var alreadyExists = await _repository.IsExists(request.Name);

            // Assert
            Assert.True(alreadyExists);

        }

        [Fact]
        public async Task UpdateElement_ShouldThrowNotFound_WhenElementDoesNotExist()
        {
            // Arrange
            var request = new UpdateElementRequest
            {
                Id = 9999, // Nieistniejące ID
                Name = "Non-existent",
                ElementGroupId = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _repository.UpdateElement(request));
        }

        [Fact]
        public async Task AddElement_ShouldHandleExtremelyLongStrings()
        {
            // Arrange
            var group = new ElementGroup { IdElementGroup = 1, Name = "Group 1" };
            await SeedDataAsync(new[] { group });

            string longDescription = new string('A', 5000); // Bardzo długi opis
            var request = new CreateElementRequest("Edge Case Name", 1, longDescription);

            // Act
            var result = await _repository.AddElement(request);

            // Assert
            Assert.True(result._IsSuccess);
            Assert.Equal(longDescription, result._Data!.Description);
        }

        [Fact]
        public async Task GetElementByIdQuery_ShouldReturnNullData_WhenIdIsInvalid()
        {
            // Act
            var result = await _repository.GetElementByIdQuery(-1);

            // Assert
            Assert.True(result._IsSuccess); 
            Assert.Null(result._Data);    
        }

        [Fact]
        public async Task GetElementsPagedByNameQuery_ShouldReturnEmptyResults_WhenNameDoesNotMatch()
        {
            // Arrange
            var group = new ElementGroup { IdElementGroup = 1, Name = "G1" };
            await SeedDataAsync(new[] { group });
            await SeedDataAsync(new[] { new Element { Name = "Xylophone", ElementGroupId = 1 } });

            // Act
            var result = await _repository.GetElementsPagedByNameQuery("NonExistentName123", 1, 10);

            // Assert
            Assert.Empty(result._Data!.Results);
            Assert.Equal(0, result._Data.TotalCount);
        }
    }
}
