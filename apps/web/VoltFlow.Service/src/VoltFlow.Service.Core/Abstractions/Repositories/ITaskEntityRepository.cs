using VoltFlow.Service.Core.Enums;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.TaskEntity.DTOs;
using VoltFlow.Service.Core.Models.TaskEntity.Request;

namespace VoltFlow.Service.Core.Abstractions.Repositories
{
    public interface ITaskEntityRepository
    {
        Task<ServiceResponse<TaskEntityDTO>> AddTaskEntity(CreateTaskEntityRequest request);
        Task<ServiceResponse<TaskEntitiesDTO>> GetTaskEntitiesQuery();
        Task<ServiceResponse<TaskEntityDTO>> GetTaskEntityByIdQuery(int id);
        Task<ServiceResponse<PagedResultDTO<TaskEntityDTO>>> GetTaskEntitySearchQuery(string? name, int page, int size);
        Task<ServiceResponse<TaskEntityDTO>> UpdateTaskEntity(UpdateTaskEntityRequest request);
        Task<bool> IsExists(string description, TaskEntityType type, int? excludeId = null);
    }
}
