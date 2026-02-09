using MediatR;
using VoltFlow.Service.Application.Commands.TaskEntity;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.TaskEntity.DTOs;

namespace VoltFlow.Service.Infrastructure.Handlers.TaskEntity
{
    public class CreateTaskEntityHandler : IRequestHandler<CreateTaskEntityCommand, ServiceResponse<TaskEntityDTO>>
    {
        private ITaskEntityService _taskEntityService;

        public CreateTaskEntityHandler(ITaskEntityService taskEntityService)
        {
            _taskEntityService = taskEntityService;
        }

        public async Task<ServiceResponse<TaskEntityDTO>> Handle(CreateTaskEntityCommand request, CancellationToken cancellationToken)
        {
            return await _taskEntityService.CreateTaskEntity(request.Request);
        }
    }
}
