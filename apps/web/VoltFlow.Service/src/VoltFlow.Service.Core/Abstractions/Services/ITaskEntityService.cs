using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.TaskEntity.DTOs;
using VoltFlow.Service.Core.Models.TaskEntity.Request;

namespace VoltFlow.Service.Core.Abstractions.Services
{
    public interface ITaskEntityService
    {
        Task<ServiceResponse<TaskEntityDTO>> CreateTaskEntity(CreateTaskEntityRequest name);

        Task<ServiceResponse<TaskEntityDTO>> UpdateTaskEntity(UpdateTaskEntityRequest request);
    }
}
