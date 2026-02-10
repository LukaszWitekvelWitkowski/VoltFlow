using Moq;
using VoltFlow.Service.Application.Services;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Exceptions;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Element.DTOs;
using VoltFlow.Service.Core.Models.Element.Request;

namespace VoltFlow.Service.Test.UnitTests.Service
{
    public class ElementServiceTests
    {
        private readonly Mock<IElementRepository> _repoMock;
        private readonly ElementService _service;

        public ElementServiceTests()
        {
            _repoMock = new Mock<IElementRepository>();
            _service = new ElementService(_repoMock.Object);
        }

        #region Create Tests

        [Fact]
        public async Task CreateElement_ShouldThrowValidationException_WhenNameIsInvalid()
        {
            // Arrange
            var request = new CreateElementRequest("", 1, "Desc");

            // Act & Assert
            await Assert.ThrowsAsync<ValidationEntityException>(() => _service.CreateElement(request));
        }

        [Fact]
        public async Task CreateElement_ShouldThrowConflict_WhenNameAlreadyExists()
        {
            // Arrange
            var request = new CreateElementRequest("Relay", 1, "Desc");
            _repoMock.Setup(r => r.IsExists("Relay", null)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => _service.CreateElement(request));
        }

        [Fact]
        public async Task CreateElement_ShouldSucceed_WhenDataIsCorrect()
        {
            // Arrange
            var request = new CreateElementRequest("New Element", 1, "Desc");
            var expectedDto = new ElementDTO { IdElement = 100, Name = "New Element" };

            _repoMock.Setup(r => r.IsExists("New Element", null)).ReturnsAsync(false);
            _repoMock.Setup(r => r.AddElement(request))
                     .ReturnsAsync(ServiceResponse<ElementDTO>.Result(expectedDto));

            // Act
            var result = await _service.CreateElement(request);

            // Assert
            Assert.True(result._IsSuccess);
            Assert.Equal(100, result._Data!.IdElement);
            _repoMock.Verify(r => r.AddElement(request), Times.Once);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task UpdateElement_ShouldReturnCurrent_WhenDescriptionIsIdenticalNullAndEmpty()
        {
            // Arrange
            // Scenariusz: W bazie jest null, w requeście przychodzi string.Empty (lub odwrotnie)
            var request = new UpdateElementRequest { Id = 1, Name = "Relay", ElementGroupId = 1, Description = "" };
            var currentDto = new ElementDTO { IdElement = 1, Name = "Relay", ElementGroupId = 1, Description = null };

            _repoMock.Setup(r => r.GetElementByIdQuery(1))
                     .ReturnsAsync(ServiceResponse<ElementDTO>.Result(currentDto));

            // Act
            var result = await _service.UpdateElement(request);

            // Assert
            Assert.True(result._IsSuccess);
            _repoMock.Verify(r => r.UpdateElement(It.IsAny<UpdateElementRequest>()), Times.Never);
        }

        [Fact]
        public async Task UpdateElement_ShouldSucceed_WhenOnlyDescriptionChanged()
        {
            // Arrange
            var request = new UpdateElementRequest { Id = 1, Name = "Relay", ElementGroupId = 1, Description = "New Desc" };
            var currentDto = new ElementDTO { IdElement = 1, Name = "Relay", ElementGroupId = 1, Description = "Old Desc" };

            _repoMock.Setup(r => r.GetElementByIdQuery(1))
                     .ReturnsAsync(ServiceResponse<ElementDTO>.Result(currentDto));
            _repoMock.Setup(r => r.IsExists("Relay", 1)).ReturnsAsync(false);
            _repoMock.Setup(r => r.UpdateElement(request))
                     .ReturnsAsync(ServiceResponse<ElementDTO>.Result(new ElementDTO { IdElement = 1, Description = "New Desc" }));

            // Act
            var result = await _service.UpdateElement(request);

            // Assert
            Assert.True(result._IsSuccess);
            _repoMock.Verify(r => r.UpdateElement(request), Times.Once);
        }

        [Fact]
        public async Task UpdateElement_ShouldThrowNotFound_WhenEntityIsMissing()
        {
            // Arrange
            var request = new UpdateElementRequest { Id = 404, Name = "Missing" };
            _repoMock.Setup(r => r.GetElementByIdQuery(404))
                     .ReturnsAsync(ServiceResponse<ElementDTO>.Result(null!));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateElement(request));
        }

        #endregion
    }
}
