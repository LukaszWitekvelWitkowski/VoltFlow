using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Enums;
using VoltFlow.Service.Core.Exceptions;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.TaskEntity.DTOs;
using VoltFlow.Service.Core.Models.TaskEntity.Request;
using VoltFlow.Service.Core.Pagination;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class TaskEntityRepository : CacheRepository<TaskEntitiesDTO, TaskEntityDTO, TaskEntity>, ITaskEntityRepository
    {
        public TaskEntityRepository(VoltFlowDbContext context, IConfiguration configuration)
            : base(context, configuration)
        {
        }

     
        public async Task<ServiceResponse<TaskEntityDTO>> AddTaskEntity(CreateTaskEntityRequest request)
        {
            var entity = new TaskEntity
            {
                Description = request.Description.Trim(),
                Status = request.Status,
                TypeTask = request.TypeTask
            };

            _context.Set<TaskEntity>().Add(entity);
            await _context.SaveChangesAsync();

            // Inwalidacja cache
            ResetStaticCache();

            return ServiceResponse<TaskEntityDTO>.Success(MapToDto(entity));
        }

        public async Task<ServiceResponse<TaskEntitiesDTO>> GetTaskEntitiesQuery()
        {
            var cache = await GetOrUpdateCacheAsync();
            if (cache != null) return ServiceResponse<TaskEntitiesDTO>.Result(cache);

            var data = await FetchFromDbInternal();
            return ServiceResponse<TaskEntitiesDTO>.Result(new TaskEntitiesDTO() { Items = data });
        }

        public async Task<ServiceResponse<TaskEntityDTO>> GetTaskEntityByIdQuery(int id)
        {
            var cache = await GetOrUpdateCacheAsync();
            if (cache != null)
            {
                var item = cache.Items.FirstOrDefault(t => t.IdTask == id);
                return ServiceResponse<TaskEntityDTO>.Result(item);
            }

            var entity = await _context.Set<TaskEntity>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.IdTask == id);

            return ServiceResponse<TaskEntityDTO>.Result(MapToDto(entity));
        }

        public async Task<ServiceResponse<PagedResultDTO<TaskEntityDTO>>> GetTaskEntitySearchQuery(string? name, int page, int size)
        {
            var cache = await GetOrUpdateCacheAsync();

            if (cache != null)
            {
                var sourceResponse = ServiceResponse<IEnumerable<TaskEntityDTO>>.Result(cache.Items);

                return PagedHelper.ToPagedResponse(
                    sourceResponse,
                    name,
                    t => t.Description, // Wybieramy pole do filtrowania
                    page,
                    size
                );
            }

            // Fallback SQL
            var dbQuery = _context.Set<TaskEntity>().AsNoTracking();
            if (!string.IsNullOrWhiteSpace(name))
            {
                var search = name.Trim().ToLower();
                dbQuery = dbQuery.Where(t => t.Description.ToLower().Contains(search));
            }

            var dbTotalCount = await dbQuery.CountAsync();
            var dbItems = await dbQuery
                .OrderBy(t => t.Description)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(t => MapToDto(t))
                .ToListAsync();

            return ServiceResponse<PagedResultDTO<TaskEntityDTO>>.Result(
                new PagedResultDTO<TaskEntityDTO>(dbItems, dbTotalCount, page, size));
        }

        public async Task<ServiceResponse<TaskEntityDTO>> UpdateTaskEntity(UpdateTaskEntityRequest request)
        {
            var entity = await _context.Set<TaskEntity>().FindAsync(request.IdTask);
            if (entity == null) throw new NotFoundException("Nie znaleziono zadania.");

            entity.Description = request.Description.Trim();
            entity.Status = request.Status;
            entity.TypeTask = request.TypeTask;

            await _context.SaveChangesAsync();
            ResetStaticCache();

            return ServiceResponse<TaskEntityDTO>.Success(MapToDto(entity));
        }

        public async Task<bool> IsExists(string description, TaskEntityType type, int? excludeId = null)
        {
            var cache = await GetOrUpdateCacheAsync();
            var normalizedDesc = description.Trim().ToLower();

            if (cache != null)
            {
                return cache.Items.Any(t => t.IdTask != excludeId
                                                 && t.TypeTask == type
                                                 && t.Description.ToLower() == normalizedDesc);
            }

            return await _context.Set<TaskEntity>()
                .AnyAsync(t => (excludeId == null || t.IdTask != excludeId)
                               && t.TypeTask == type
                               && t.Description.ToLower() == normalizedDesc);
        }

      
        override
        public TaskEntityDTO MapToDto(TaskEntity t) => new TaskEntityDTO
        {
            IdTask = t.IdTask,
            Description = t.Description,
            Status = t.Status,
            TypeTask = t.TypeTask
        };
    }
}
