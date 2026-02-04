using MediatR;
using VoltFlow.Service.Core.Models;
using VoltFlow.Service.Core.Models.Element;

namespace VoltFlow.Service.Application.Queries.Element
{
    public class GetElementsQuery : IRequest<ServiceResponse<ElementsDTO>>
    {
    }
}
