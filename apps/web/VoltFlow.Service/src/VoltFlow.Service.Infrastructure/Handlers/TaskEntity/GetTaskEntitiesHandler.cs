using MediatR;
using VoltFlow.Service.Application.Queries.TaskEntity;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.TaskEntity.DTOs;
using VoltFlow.Service.Core.Models.Validators;

namespace VoltFlow.Service.Infrastructure.Handlers.TaskEntity
{
    public class GetTaskEntitiesHandler : IRequestHandler<GetTaskEntitiesQuery, ServiceResponse<TaskEntitiesDTO>>
    {
        private readonly ITaskEntityRepository _taskEntityRepository;
        public GetTaskEntitiesHandler(ITaskEntityRepository taskEntityRepository)
        {
            _taskEntityRepository = taskEntityRepository;
        }
        public async Task<ServiceResponse<TaskEntitiesDTO>> Handle(GetTaskEntitiesQuery request, CancellationToken cancellationToken)
        {
            return await ResponseValidator.ExecuteAsync( async () => await _taskEntityRepository.GetTaskEntitiesQuery(), "Task Entity");
        }
    }
}
