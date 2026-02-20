using Microsoft.Extensions.Configuration;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Role.DTOs;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class RoleRespository : CacheRepository<RolesDTO,RoleDto, Role>, IRoleRepository
    {
        public RoleRespository(VoltFlowDbContext context, IConfiguration configuration) : base(context, configuration)
        {
        }

        override
        public RoleDto MapToDto(Role e) => new RoleDto
        {
            Id = e.IdRole,
            Name = e.Name
        };

        public async Task<ServiceResponse<RolesDTO>> GetRolesQuery()
        {
            var cache = await GetOrUpdateCacheAsync();

            if (cache != null)
                return ServiceResponse<RolesDTO>.Result(_cache);

            var dbData = await FetchFromDbInternal();
            return ServiceResponse<RolesDTO>.Result(new RolesDTO { Items = dbData });
        }
    }
}
