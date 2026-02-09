using MediatR;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.TaskEntity.DTOs;

namespace VoltFlow.Service.Application.Queries.TaskEntity
{
    public class GetTaskEntitiesQuery : IRequest<ServiceResponse<TaskEntitiesDTO>>
    {
    }
}
