using MediatR;
using VoltFlow.Service.Core.Models;
using VoltFlow.Service.Core.Models.Element;

namespace VoltFlow.Service.Application.Queries.Element
{
    public class GetElementByNameQuery : IRequest<ServiceResponse<ElementDTO>>
    {
        public string Name { get; }
        public GetElementByNameQuery(string name)
        {
            Name = name;
        }
    }
}
