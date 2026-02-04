using MediatR;
using VoltFlow.Service.Core.Models;
using VoltFlow.Service.Core.Models.Element;

namespace VoltFlow.Service.Application.Queries.Element
{
    public class GetElementByIdQuery : IRequest<ServiceResponse<ElementDTO>>
    {
        public int Id { get; }
        public GetElementByIdQuery(int id)
        {
            Id = id;
        }
    }
}
