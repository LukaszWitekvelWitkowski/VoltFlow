using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Role.DTOs;

namespace VoltFlow.Service.Core.Abstractions.Repositories
{
    public interface IRoleRepository
    {
        Task<ServiceResponse<RolesDTO>> GetRolesQuery();
    }
}
