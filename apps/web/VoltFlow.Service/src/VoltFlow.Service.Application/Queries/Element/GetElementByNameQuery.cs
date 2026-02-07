using MediatR;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Element.DTOs;

namespace VoltFlow.Service.Application.Queries.Element
{
    public class GetElementByNameQuery : PaginationParams, IRequest<ServiceResponse<PagedResultDTO<ElementDTO>>> 
    {
        public string? Name { get; }
        public GetElementByNameQuery(string? name, int number, int size)
        {
            Name = name;
            PageNumber = number;
            PageSize = size;
        }
    }
}
