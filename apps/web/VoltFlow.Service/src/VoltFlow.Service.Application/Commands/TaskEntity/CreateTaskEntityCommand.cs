using MediatR;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.TaskEntity.DTOs;
using VoltFlow.Service.Core.Models.TaskEntity.Request;

namespace VoltFlow.Service.Application.Commands.TaskEntity
{
    public class CreateTaskEntityCommand : IRequest<ServiceResponse<TaskEntityDTO>>
    {
        public CreateTaskEntityCommand(CreateTaskEntityRequest request)
        {
            Request = request;
        }
        public CreateTaskEntityRequest Request { get; }
    }
}
