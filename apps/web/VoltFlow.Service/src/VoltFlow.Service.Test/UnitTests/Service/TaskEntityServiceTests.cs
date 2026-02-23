using Moq;
using VoltFlow.Service.Application.Services;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Enums;
using VoltFlow.Service.Core.Exceptions;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.TaskEntity.DTOs;
using VoltFlow.Service.Core.Models.TaskEntity.Request;

namespace VoltFlow.Service.Test.UnitTests.Service
{
    public class TaskEntityServiceTests
    {
        private readonly Mock<ITaskEntityRepository> _taskRepoMock;
        private readonly TaskEntityService _service;

        public TaskEntityServiceTests()
        {
            _taskRepoMock = new Mock<ITaskEntityRepository>();
            _service = new TaskEntityService(_taskRepoMock.Object);
        }

        #region Create Tests

        [Fact]
        public async Task CreateTaskEntity_ShouldThrowConflict_WhenTaskAlreadyExistsForType()
        {
            // Arrange
            var request = new CreateTaskEntityRequest { Description = "Maintenance", TypeTask = TaskEntityType.Maintenance };

            // Symulujemy, że IsExists znajduje duplikat dla tego konkretnego typu
            _taskRepoMock.Setup(r => r.IsExists("Maintenance", TaskEntityType.Maintenance, null))
                         .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => _service.CreateTaskEntity(request));
        }

        [Fact]
        public async Task CreateTaskEntity_ShouldSucceed_WhenDataIsUnique()
        {
            // Arrange
            var request = new CreateTaskEntityRequest { Description = "New Job", TypeTask = TaskEntityType.Installation };
            var expectedDto = new TaskEntityDTO { IdTask = 1, Description = "New Job" };

            _taskRepoMock.Setup(r => r.IsExists(request.Description, request.TypeTask, null))
                         .ReturnsAsync(false);
            _taskRepoMock.Setup(r => r.AddTaskEntity(request))
                         .ReturnsAsync(expectedDto);

            // Act
            var result = await _service.CreateTaskEntity(request);

            // Assert
            Assert.True(result._IsSuccess);
            Assert.Equal("New Job", result._Data!.Description);
            _taskRepoMock.Verify(r => r.AddTaskEntity(request), Times.Once);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task UpdateTaskEntity_ShouldBeIdempotent_WhenDataHasNoChanges()
        {
            // Arrange
            var request = new UpdateTaskEntityRequest
            {
                IdTask = 5,
                Description = "Fix It",
                Status = WorkItemStatus.ToDo,
                TypeTask = TaskEntityType.Installation
            };

            var currentDto = new TaskEntityDTO
            {
                IdTask = 5,
                Description = "  Fix It  ",
                Status = WorkItemStatus.ToDo,
                TypeTask = TaskEntityType.Installation
            };

            _taskRepoMock.Setup(r => r.GetTaskEntityByIdQuery(5))
                         .ReturnsAsync(currentDto);

            // Act
            var result = await _service.UpdateTaskEntity(request);

            // Assert
            Assert.True(result._IsSuccess);

            _taskRepoMock.Verify(r => r.UpdateTaskEntity(It.IsAny<UpdateTaskEntityRequest>()), Times.Never);
        }

        [Fact]
        public async Task UpdateTaskEntity_ShouldThrowNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            var request = new UpdateTaskEntityRequest { IdTask = 999 };
            _taskRepoMock.Setup(r => r.GetTaskEntityByIdQuery(999))
                         .ReturnsAsync((TaskEntityDTO)null!);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateTaskEntity(request));
        }

        [Fact]
        public async Task UpdateTaskEntity_ShouldSucceed_WhenStatusChanges()
        {
            // Arrange
            var request = new UpdateTaskEntityRequest { IdTask = 1, Description = "Test", Status = WorkItemStatus.Done };
            var currentDto = new TaskEntityDTO { IdTask = 1, Description = "Test", Status = WorkItemStatus.ToDo };

            _taskRepoMock.Setup(r => r.GetTaskEntityByIdQuery(1))
                         .ReturnsAsync(currentDto);
            _taskRepoMock.Setup(r => r.UpdateTaskEntity(request))
                         .ReturnsAsync(new TaskEntityDTO { Status = WorkItemStatus.Done });

            // Act
            var result = await _service.UpdateTaskEntity(request);

            // Assert
            Assert.True(result._IsSuccess);
            _taskRepoMock.Verify(r => r.UpdateTaskEntity(request), Times.Once);
        }

        #endregion
    }
}
