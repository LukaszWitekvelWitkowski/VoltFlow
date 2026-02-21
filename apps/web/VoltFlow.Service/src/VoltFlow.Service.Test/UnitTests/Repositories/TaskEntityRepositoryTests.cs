using Microsoft.EntityFrameworkCore;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Enums;
using VoltFlow.Service.Core.Models.TaskEntity.DTOs;
using VoltFlow.Service.Core.Models.TaskEntity.Request;
using VoltFlow.Service.Infrastructure.Repositories;

namespace VoltFlow.Service.Test.UnitTests.Repositories
{
    public class TaskEntityRepositoryTests : VoltFlowTestBase
    {
        private readonly TaskEntityRepository _repository;

        public TaskEntityRepositoryTests() : base()
        {
            _repository = new TaskEntityRepository(_context, _configuration);
        }

        [Fact]
        public async Task AddTaskEntity_ShouldSaveToDb_AndReturnCorrectDto()
        {
            // Arrange
            var request = new CreateTaskEntityRequest
            {
                Description = "  Fix cables  ",
                Status = WorkItemStatus.ToDo,
                TypeTask = TaskEntityType.Installation
            };

            // Act
            var result = await _repository.AddTaskEntity(request);

            // Assert
            Assert.True(result._IsSuccess);
            Assert.Equal("Fix cables", result._Data!.Description);
            Assert.Equal(1, await _context.Set<TaskEntity>().CountAsync());
        }

        [Fact]
        public async Task GetTaskEntitySearchQuery_ShouldFilterByDescription()
        {
            CacheRepository<TaskEntitiesDTO, TaskEntityDTO, TaskEntity>.ResetStaticCache();
            // Arrange
            var tasks = new List<TaskEntity>
            {
                new TaskEntity { IdTask = 1, Description = "Mounting", TypeTask = TaskEntityType.Installation },
                new TaskEntity { IdTask = 2, Description = "Maintenance", TypeTask = TaskEntityType.Maintenance },
                new TaskEntity { IdTask = 3, Description = "Final Check", TypeTask = TaskEntityType.Installation }
            };
            await SeedDataAsync(tasks);

            // Act
            var result = await _repository.GetTaskEntitySearchQuery("mount", 1, 10);

            // Assert
            Assert.Single(result._Data!.Results);
            Assert.Equal("Mounting", result._Data.Results.First().Description);
            Assert.Equal(1, result._Data.TotalCount);
        }

        [Fact]
        public async Task UpdateTaskEntity_ShouldModifyExistingRecord_AndInvalidateCache()
        {
            // Arrange
            var task = new TaskEntity { IdTask = 5, Description = "Old Desc", Status = WorkItemStatus.ToDo };
            await SeedDataAsync(new[] { task });

            // cache Initialization 
            await _repository.GetTaskEntitiesQuery();

            var request = new UpdateTaskEntityRequest
            {
                IdTask = 5,
                Description = "New Desc",
                Status = WorkItemStatus.ToDo
            };

            // Act
            await _repository.UpdateTaskEntity(request);
            var updatedResult = await _repository.GetTaskEntitiesQuery();

            // Assert
            var updatedItem = updatedResult._Data!.Items.First(t => t.IdTask == 5);
            Assert.Equal("New Desc", updatedItem.Description);
            Assert.Equal(WorkItemStatus.ToDo, updatedItem.Status);
        }

        [Fact]
        public async Task IsExists_ShouldCheckDescriptionAndType()
        {
            CacheRepository<TaskEntitiesDTO, TaskEntityDTO, TaskEntity>.ResetStaticCache();
            // Arrange
            var task = new TaskEntity { IdTask = 1, Description = "Test", TypeTask = TaskEntityType.Installation };
            await SeedDataAsync(new[] { task });

            // Act
     
            var exists = await _repository.IsExists("test", TaskEntityType.Installation);
       
            var existsDifferentType = await _repository.IsExists("test", TaskEntityType.Maintenance);
  
            var existsExcludeSelf = await _repository.IsExists("test", TaskEntityType.Installation, 1);

            // Assert
            Assert.True(exists);
            Assert.False(existsDifferentType);
            Assert.False(existsExcludeSelf);
        }

        [Fact]
        public async Task GetTaskEntityByIdQuery_ShouldReturnNull_WhenEntityDoesNotExist()
        {
            CacheRepository<TaskEntitiesDTO, TaskEntityDTO, TaskEntity>.ResetStaticCache();
            // Arrange

            // Act
            var result = await _repository.GetTaskEntityByIdQuery(-1);

            // Assert
            Assert.Null(result._Data);
        }

        [Fact]
        public async Task GetTaskEntityByIdQuery_ShouldReturnNull_WhenTaskNotFound()
        {
            CacheRepository<TaskEntitiesDTO, TaskEntityDTO, TaskEntity>.ResetStaticCache();
            // Act
            var result = await _repository.GetTaskEntityByIdQuery(99999);

            // Assert
            Assert.Null(result._Data);
        }

        [Fact]
        public async Task IsExists_ShouldReturnFalse_WhenSearchingWithEmptyString()
        {
            CacheRepository<TaskEntitiesDTO, TaskEntityDTO, TaskEntity>.ResetStaticCache();
            // Act
            var exists = await _repository.IsExists("", TaskEntityType.Installation);

            // Assert
            Assert.False(exists);
        }
    }
}
