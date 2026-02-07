using MediatR;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Element.DTOs;

namespace VoltFlow.Service.Application.Queries.Element
{
    public class GetElementsQuery : IRequest<ServiceResponse<ElementsDTO>>
    {
    }
}
