using MediatR;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;

namespace VoltFlow.Service.Application.Queries.ElementGroup
{
    public class GetElementGroupsQuery : IRequest<ServiceResponse<ElementGroupCacheDTO>>
    {
    }
}
