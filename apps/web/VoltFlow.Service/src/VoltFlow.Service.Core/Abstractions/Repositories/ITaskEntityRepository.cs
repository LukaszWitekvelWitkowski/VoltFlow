using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.TaskEntity.DTOs;

namespace VoltFlow.Service.Core.Abstractions.Repositories
{
    public interface ITaskEntityRepository
    {
        Task<ServiceResponse<TaskEntitiesDTO>> GetTaskEntitiesQuery();
        Task<ServiceResponse<TaskEntityDTO>> GetTaskEntityByIdQuery(int id);
        Task<ServiceResponse<PagedResultDTO<TaskEntityDTO>>> GetTaskEntitySearchQuery(string? name, int page, int size);

    }
}
