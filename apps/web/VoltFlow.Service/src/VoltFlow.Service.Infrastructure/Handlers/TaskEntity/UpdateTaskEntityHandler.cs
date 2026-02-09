using MediatR;
using VoltFlow.Service.Application.Commands.TaskEntity;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.TaskEntity.DTOs;

namespace VoltFlow.Service.Infrastructure.Handlers.TaskEntity
{
    public class UpdateTaskEntityHandler : IRequestHandler<UpdateTaskEntityCommand, ServiceResponse<TaskEntityDTO>>
    {
        private readonly ITaskEntityService _service;
        public UpdateTaskEntityHandler(ITaskEntityService service)
        {
            _service = service;
        }
        public async Task<ServiceResponse<TaskEntityDTO>> Handle(UpdateTaskEntityCommand request, CancellationToken cancellationToken)
        {
            return await _service.UpdateTaskEntity(request._request);
        }
    }
}
