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

     
        public async Task<TaskEntityDTO> AddTaskEntity(CreateTaskEntityRequest request)
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

            return MapToDto(entity);
        }

        public async Task<TaskEntitiesDTO> GetTaskEntitiesQuery()
        {
            var cache = await GetOrUpdateCacheAsync();
            if (cache != null) return cache;

            var data = await FetchFromDbInternal();
            return new TaskEntitiesDTO() { Items = data };
        }

        public async Task<TaskEntityDTO?> GetTaskEntityByIdQuery(int id)
        {
            var cache = await GetOrUpdateCacheAsync();
            if (cache != null)
            {
                return cache.Items.FirstOrDefault(t => t.IdTask == id);
            }

           return await _context.Set<TaskEntity>()
                                .AsNoTracking()
                                .Where(t => t.IdTask == id)
                                .Select(t => MapToDto(t))
                                .FirstOrDefaultAsync();
        }

        public async Task<PagedResultDTO<TaskEntityDTO>> GetTaskEntitySearchQuery(string? name, int page, int size)
        {
            var cache = await GetOrUpdateCacheAsync();

            if (cache != null)
            {

                return PagedHelper.ToPagedResponse(
                    cache,
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

            return new PagedResultDTO<TaskEntityDTO>(dbItems, dbTotalCount, page, size);
        }

        public async Task<TaskEntityDTO> UpdateTaskEntity(UpdateTaskEntityRequest request)
        {
            var entity = await _context.Set<TaskEntity>().FindAsync(request.IdTask);
            if (entity == null) throw new NotFoundException("Nie znaleziono zadania.");

            entity.Description = request.Description.Trim();
            entity.Status = request.Status;
            entity.TypeTask = request.TypeTask;

            await _context.SaveChangesAsync();
            ResetStaticCache();

            return MapToDto(entity);
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
