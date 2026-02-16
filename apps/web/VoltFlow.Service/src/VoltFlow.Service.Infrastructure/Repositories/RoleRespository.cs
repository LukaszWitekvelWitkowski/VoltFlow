using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Role.DTOs;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class RoleRespository : BaseRepository, IRoleRepository
    {
        private RolesDTO? _cache;
        public RoleRespository(VoltFlowDbContext context, IConfiguration configuration) : base(context, configuration)
        {
        }

        private async Task<RolesDTO> GetOrUpdateCacheAsync()
        {
            if (!_isCacheEnabled) return null!;

            if (_cache != null) return _cache;

            await _lock.WaitAsync();
            try
            {
                if (_cache == null)
                {
                    var count = await _context.Set<Role>().CountAsync();
                    if (count > _maxCacheThreshold)
                    {
                        _isCacheEnabled = false;
                        return null!;
                    }

                    var data = await FetchFromDbInternal();
                    _cache = new RolesDTO { Roles = data };
                }
                return _cache;
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task<IEnumerable<RoleDto>> FetchFromDbInternal()
        {
            return await _context.Set<Role>()
                .AsNoTracking()
                .Select(e => MapToDto(e))
                .ToListAsync();
        }


        private static RoleDto MapToDto(Role e) => new RoleDto
        {
            Id = e.IdRole,
            Name = e.Name
        };

        public async Task<ServiceResponse<RolesDTO>> GetRolesQuery()
        {
            var cache = await GetOrUpdateCacheAsync();

            if (cache != null)
                return ServiceResponse<RolesDTO>.Result(cache);

            var dbData = await FetchFromDbInternal();
            return ServiceResponse<RolesDTO>.Result(new RolesDTO { Roles = dbData });
        }
    }
}
