using MediatR;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Role.DTOs;

namespace VoltFlow.Service.Application.Queries.Role
{
    public class GetRolesQuery : IRequest<ServiceResponse<RolesDTO>>
    {
    }
}
