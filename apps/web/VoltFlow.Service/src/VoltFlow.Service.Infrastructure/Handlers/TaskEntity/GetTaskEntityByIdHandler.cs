using MediatR;
using VoltFlow.Service.Application.Queries.TaskEntity;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.TaskEntity.DTOs;
using VoltFlow.Service.Core.Models.Validators;

namespace VoltFlow.Service.Infrastructure.Handlers.TaskEntity
{
    public class GetTaskEntityByIdHandler : IRequestHandler<GetTaskEntityByIdQuery, ServiceResponse<TaskEntityDTO>>
    {
        private readonly ITaskEntityRepository _taskEntityRepository;

        public GetTaskEntityByIdHandler(ITaskEntityRepository taskEntityRepository)
        {
            _taskEntityRepository = taskEntityRepository;
        }

        public async Task<ServiceResponse<TaskEntityDTO>> Handle(GetTaskEntityByIdQuery request, CancellationToken cancellationToken)
        {
            return ResponseValidator.EnsureSuccessAndData(await _taskEntityRepository.GetTaskEntityByIdQuery(request.Id), "Task Entity");
        }
    }
}
