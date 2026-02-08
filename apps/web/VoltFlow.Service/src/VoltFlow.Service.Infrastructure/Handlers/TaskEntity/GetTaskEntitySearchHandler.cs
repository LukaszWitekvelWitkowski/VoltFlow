using MediatR;
using VoltFlow.Service.Application.Queries.TaskEntity;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.TaskEntity.DTOs;
using VoltFlow.Service.Core.Models.Validators;

namespace VoltFlow.Service.Infrastructure.Handlers.TaskEntity
{
    public class GetTaskEntitySearchHandler : IRequestHandler<GetTaskEntitySearchQuery, ServiceResponse<PagedResultDTO<TaskEntityDTO>>>
    {
        private readonly ITaskEntityRepository _taskEntityRepository;

        public GetTaskEntitySearchHandler(ITaskEntityRepository taskEntityRepository)
        {
            _taskEntityRepository = taskEntityRepository;
        }

        public async Task<ServiceResponse<PagedResultDTO<TaskEntityDTO>>> Handle(GetTaskEntitySearchQuery request, CancellationToken cancellationToken)
        {
            return ResponseValidator.EnsureSuccessAndData(await _taskEntityRepository.GetTaskEntitySearchQuery(request.Name, request.PageNumber, request.PageSize), "Task Entity");
        }
    }
}
