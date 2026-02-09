using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Exceptions;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.TaskEntity.DTOs;
using VoltFlow.Service.Core.Models.TaskEntity.Request;

namespace VoltFlow.Service.Application.Services
{
    public class TaskEntityService : ITaskEntityService
    {
        private readonly ITaskEntityRepository _taskRepository;

        public TaskEntityService(ITaskEntityRepository taskRepository) => _taskRepository = taskRepository;

        public async Task<ServiceResponse<TaskEntityDTO>> CreateTaskEntity(CreateTaskEntityRequest request)
        {
            if (string.IsNullOrEmpty(request.Description))
                throw new ValidationEntityException("Opis nie może być pusty.");

            if (await _taskRepository.IsExists(request.Description, request.TypeTask))
                throw new ConflictException("Takie zadanie już istnieje.");

            var result = await _taskRepository.AddTaskEntity(request);
            return ServiceResponse<TaskEntityDTO>.Success(result._Data!); // Repozytorium może nadal zwracać ServiceResponse dla spójności
        }

        public async Task<ServiceResponse<TaskEntityDTO>> UpdateTaskEntity(UpdateTaskEntityRequest request)
        {
            // 1. Download (Repository will throw NotFound if it doesn't find it)
            var current = (await _taskRepository.GetTaskEntityByIdQuery(request.IdTask))._Data
                          ?? throw new NotFoundException("Zadanie nie istnieje.");

            // 2. Checking if it makes sense to update
            if (IsDataUnchanged(current, request))
                return ServiceResponse<TaskEntityDTO>.Success(current);

            // 3. Duplicate validation
            if (await _taskRepository.IsExists(request.Description, request.TypeTask, request.IdTask))
                throw new ConflictException("Istnieje już inne zadanie o tym opisie.");

            var updated = await _taskRepository.UpdateTaskEntity(request);
            return ServiceResponse<TaskEntityDTO>.Success(updated._Data!);
        }

        private bool IsDataUnchanged(TaskEntityDTO c, UpdateTaskEntityRequest r) =>
            c.Description.Trim().Equals(r.Description.Trim(), StringComparison.OrdinalIgnoreCase) &&
            c.Status == r.Status && c.TypeTask == r.TypeTask;
    }
}
