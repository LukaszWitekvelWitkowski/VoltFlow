using Moq;
using VoltFlow.Service.Application.Services;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Exceptions;
using VoltFlow.Service.Core.Models.Category.DTOs;
using VoltFlow.Service.Core.Models.Category.Request;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Test.UnitTests.Service
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepoMock;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _categoryRepoMock = new Mock<ICategoryRepository>();
            _service = new CategoryService(_categoryRepoMock.Object);
        }

        #region CreateCategory Tests

        [Fact]
        public async Task CreateCategory_ShouldThrowValidationException_WhenNameIsEmpty()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ValidationEntityException>(() => _service.CreateCategory(""));
        }

        [Fact]
        public async Task CreateCategory_ShouldThrowConflictException_WhenNameAlreadyExists()
        {
            // Arrange
            _categoryRepoMock.Setup(r => r.IsExists("Electronics", null))
                             .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => _service.CreateCategory("Electronics"));
        }

        [Fact]
        public async Task CreateCategory_ShouldSucceed_WhenDataIsValid()
        {
            // Arrange
            var categoryName = "New Category";
            var expectedDto = new CategoryDTO { Id = 1, Name = categoryName };

            _categoryRepoMock.Setup(r => r.IsExists(categoryName, null)).ReturnsAsync(false);
            _categoryRepoMock.Setup(r => r.AddCategory(categoryName))
                             .ReturnsAsync(expectedDto);

            // Act
            var result = await _service.CreateCategory(categoryName);

            // Assert
            Assert.True(result._IsSuccess);
            Assert.Equal(categoryName, result._Data!.Name);
            _categoryRepoMock.Verify(r => r.AddCategory(categoryName), Times.Once);
        }

        #endregion

        #region UpdateCategory Tests

        [Fact]
        public async Task UpdateCategory_ShouldReturnCurrentData_WhenDataIsUnchanged()
        {
            // Arrange
            var request = new UpdateCategoryRequest { Id = 1, Name = "Same", IsObsolete = false };
            var currentDto = new CategoryDTO { Id = 1, Name = "Same", IsObsolete = false };

            _categoryRepoMock.Setup(r => r.GetCategoryByIdQuery(1))
                             .ReturnsAsync(currentDto);

            // Act
            var result = await _service.UpdateCategory(request);

            // Assert
            Assert.True(result._IsSuccess);
            Assert.Equal(currentDto, result._Data);
            // Kluczowe: Verify sprawia, że upewniamy się, iż UpdateCategory w repo NIE zostało wywołane (idempotentność)
            _categoryRepoMock.Verify(r => r.UpdateCategory(It.IsAny<UpdateCategoryRequest>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCategory_ShouldThrowNotFound_WhenCategoryDoesNotExistInDb()
        {
            // Arrange
            var request = new UpdateCategoryRequest { Id = 99, Name = "NonExistent" };

            // Symulujemy zwrócenie null przez repozytorium (zgodnie z naszą ostatnią poprawką!)
            _categoryRepoMock.Setup(r => r.GetCategoryByIdQuery(99))
                             .ReturnsAsync((CategoryDTO)null!);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateCategory(request));
        }

        [Fact]
        public async Task UpdateCategory_ShouldThrowConflict_WhenNewNameIsTakenByOtherCategory()
        {
            // Arrange
            var request = new UpdateCategoryRequest { Id = 1, Name = "ExistingName" };
            var currentDto = new CategoryDTO { Id = 1, Name = "OldName" };

            _categoryRepoMock.Setup(r => r.GetCategoryByIdQuery(1))
                             .ReturnsAsync(currentDto);
            _categoryRepoMock.Setup(r => r.IsExists("ExistingName", 1))
                             .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => _service.UpdateCategory(request));
        }

        #endregion
    }
}
