using Microsoft.EntityFrameworkCore;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.TaskEntity.DTOs;
using VoltFlow.Service.Core.Pagination;
using VoltFlow.Service.Core.Tools;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class TaskEntityRepository : ITaskEntityRepository
    {
        private readonly VoltFlowDbContext _context;
        private readonly LazyValue<ServiceResponse<TaskEntitiesDTO>> _allTaskEntityLazy;

        public TaskEntityRepository(VoltFlowDbContext context)
        {
            _context = context;
            // Inicjalizacja leniwego ładowania dla zadań
            _allTaskEntityLazy = new LazyValue<ServiceResponse<TaskEntitiesDTO>>(FetchTaskEntitiesFromDb);
        }

        public async Task<ServiceResponse<TaskEntitiesDTO>> GetTaskEntitiesQuery()
        {
            var response = await _allTaskEntityLazy.GetValueAsync();

            // Zwracamy dane opakowane w standardowy wynik sukcesu
            return ServiceResponse<TaskEntitiesDTO>.Result(response._Data);
        }

        public async Task<ServiceResponse<TaskEntityDTO>> GetTaskEntityByIdQuery(int id)
        {
            var response = await _allTaskEntityLazy.GetValueAsync();

            if (!response._IsSuccess)
            {
                return ServiceResponse<TaskEntityDTO>.Failure(response._Message, response._StatusCode);
            }

            // Ekstrakcja konkretnego zadania z pamięci RAM
            var taskEntity = response._Data?.TaskEntities.FirstOrDefault(t => t.IdTask == id);

            return ServiceResponse<TaskEntityDTO>.Result(taskEntity);
        }

        public async Task<ServiceResponse<PagedResultDTO<TaskEntityDTO>>> GetTaskEntitySearchQuery(string? name, int page, int size)
        {
            var response = await _allTaskEntityLazy.GetValueAsync();

            if (!response._IsSuccess)
            {
                return ServiceResponse<PagedResultDTO<TaskEntityDTO>>.Failure(response._Message, response._StatusCode);
            }

            // Konwersja na listę pod PagedHelper
            var listResponse = ServiceResponse<List<TaskEntityDTO>>.Result(
                response._Data?.TaskEntities.ToList() ?? new List<TaskEntityDTO>()
            );

            return PagedHelper.ToPagedResponse(
                listResponse,
                name,
                t => t.Description, // Zakładam, że filtrujemy po właściwości Name
                page,
                size
            );
        }

        private async Task<ServiceResponse<TaskEntitiesDTO>> FetchTaskEntitiesFromDb()
        {
            try
            {
                // Pobieramy dane z bazy bez śledzenia zmian (AsNoTracking) dla wydajności
                var taskList = await _context.Set<TaskEntity>()
                    .AsNoTracking()
                    .Select(t => new TaskEntityDTO
                    {
                        IdTask = t.IdTask,
                        Description = t.Description,
                        Status = t.Status,
                        TypeTask = t.TypeTask
                    })
                    .ToListAsync();

                return ServiceResponse<TaskEntitiesDTO>.Result(new TaskEntitiesDTO(taskList));
            }
            catch (Exception ex)
            {
                // Senior Tip: Warto tu wstrzyknąć ILogger i zalogować szczegóły błędu: _logger.LogError(ex, "Db Error");
                return ServiceResponse<TaskEntitiesDTO>.Failure("Wystąpił błąd podczas pobierania zadań z bazy danych.", 500);
            }
        }
    }
}
