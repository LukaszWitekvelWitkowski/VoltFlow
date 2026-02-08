using MediatR;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;

namespace VoltFlow.Service.Application.Queries.ElementGroup
{
    public class GetElementGroupByIdQuery : IRequest<ServiceResponse<ElementGroupDTO>>
    {
        public int Id { get; }

        public GetElementGroupByIdQuery(int id)
        {
            Id = id;
        }

    }
}
