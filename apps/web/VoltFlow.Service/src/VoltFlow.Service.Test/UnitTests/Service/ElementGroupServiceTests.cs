using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using VoltFlow.Service.Application.Services;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Exceptions;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;
using VoltFlow.Service.Core.Models.ElementGroup.Request;

namespace VoltFlow.Service.Test.UnitTests.Service
{
    public class ElementGroupServiceTests
    {
        private readonly Mock<IElementGroupRepository> _repoMock;
        private readonly ElementGroupService _service;

        public ElementGroupServiceTests()
        {
            _repoMock = new Mock<IElementGroupRepository>();
            _service = new ElementGroupService(_repoMock.Object);
        }

        #region Create Tests

        [Fact]
        public async Task CreateElementGroup_ShouldThrowValidationException_WhenNameIsWhiteSpace()
        {
            // Arrange
            var request = new CreateElementGroupRequest("   ", 1);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationEntityException>(() => _service.CreateElementGroup(request));
        }

        [Fact]
        public async Task CreateElementGroup_ShouldThrowConflict_WhenNameAlreadyExists()
        {
            // Arrange
            var request = new CreateElementGroupRequest("Cables", 1);
            _repoMock.Setup(r => r.IsExists("Cables", null)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => _service.CreateElementGroup(request));
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task UpdateElementGroup_ShouldReturnCurrentData_WhenNothingChanged()
        {
            // Arrange
            var request = new UpdateElementGroupRequest
            {
                IdElementGroup = 10,
                Name = "Sensors",
                CategoryId = 1,
                IsObsolete = false
            };

            var currentDto = new ElementGroupDTO
            {
                IdElementGroup = 10,
                Name = "Sensors",
                CategoryId = 1,
                IsObsolete = false
            };

            _repoMock.Setup(r => r.GetElementGroupByIdQuery(10))
                     .ReturnsAsync(ServiceResponse<ElementGroupDTO>.Result(currentDto));

            // Act
            var result = await _service.UpdateElementGroup(request);

            // Assert
            Assert.True(result._IsSuccess);
            Assert.Equal(currentDto, result._Data);
            // Sprawdzamy, czy repozytorium NIE zostało zawołane do zapisu
            _repoMock.Verify(r => r.UpdateElementGroup(It.IsAny<UpdateElementGroupRequest>()), Times.Never);
        }

        [Fact]
        public async Task UpdateElementGroup_ShouldSucceed_WhenCategoryChangedButNameStayed()
        {
            // Arrange
            var request = new UpdateElementGroupRequest { IdElementGroup = 10, Name = "Sensors", CategoryId = 2 };
            var currentDto = new ElementGroupDTO { IdElementGroup = 10, Name = "Sensors", CategoryId = 1 };

            _repoMock.Setup(r => r.GetElementGroupByIdQuery(10))
                     .ReturnsAsync(ServiceResponse<ElementGroupDTO>.Result(currentDto));

            _repoMock.Setup(r => r.UpdateElementGroup(request))
                     .ReturnsAsync(ServiceResponse<ElementGroupDTO>.Result(new ElementGroupDTO { Name = "Sensors", CategoryId = 2 }));

            // Act
            var result = await _service.UpdateElementGroup(request);

            // Assert
            Assert.True(result._IsSuccess);
            _repoMock.Verify(r => r.UpdateElementGroup(request), Times.Once);
        }

        [Fact]
        public async Task UpdateElementGroup_ShouldThrowNotFound_WhenIdIsInvalid()
        {
            // Arrange
            var request = new UpdateElementGroupRequest { IdElementGroup = 999, Name = "Test" };

            // Symulujemy zwrócenie null, co wyzwoli NotFoundException w serwisie
            _repoMock.Setup(r => r.GetElementGroupByIdQuery(999))
                     .ReturnsAsync(ServiceResponse<ElementGroupDTO>.Result(null!));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateElementGroup(request));
        }

        #endregion
    }
}
