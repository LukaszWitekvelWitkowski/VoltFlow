using MediatR;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.TaskEntity.DTOs;
using VoltFlow.Service.Core.Models.TaskEntity.Request;

namespace VoltFlow.Service.Application.Commands.TaskEntity
{
    public class UpdateTaskEntityCommand : IRequest<ServiceResponse<TaskEntityDTO>>
    {
            public UpdateTaskEntityCommand(UpdateTaskEntityRequest request)
            {
                _request = request;
            }
            public UpdateTaskEntityRequest _request { get; }
    }
}
