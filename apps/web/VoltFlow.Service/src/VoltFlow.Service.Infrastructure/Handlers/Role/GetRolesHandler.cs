using MediatR;
using VoltFlow.Service.Application.Queries.Role;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Role.DTOs;

namespace VoltFlow.Service.Infrastructure.Handlers.Role
{
    public class GetRolesHandler : IRequestHandler<GetRolesQuery, ServiceResponse<RoleCacheDTO>>
    {
        private readonly IRoleRepository _roleRepository;
        public GetRolesHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task<ServiceResponse<RoleCacheDTO>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            return await _roleRepository.GetRolesQuery();

        }
    }
}
