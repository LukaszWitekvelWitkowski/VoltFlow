using MediatR;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;

namespace VoltFlow.Service.Application.Queries.ElementGroup
{
    public class GetElementGroupSearchQuery : PaginationParams, IRequest<ServiceResponse<PagedResultDTO<ElementGroupDTO>>>
    {
        public string? Name { get; }

        public GetElementGroupSearchQuery(string? name, int number, int size)
        {
            Name = name;
            PageNumber = number;
            PageSize = size;
        }

    }
}
