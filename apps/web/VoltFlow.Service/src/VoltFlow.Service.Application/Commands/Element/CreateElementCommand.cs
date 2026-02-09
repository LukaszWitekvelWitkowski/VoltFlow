using MediatR;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Element.DTOs;
using VoltFlow.Service.Core.Models.Element.Request;

namespace VoltFlow.Service.Application.Commands.Element
{
    public class CreateElementCommand :IRequest<ServiceResponse<ElementDTO>>
    {
        public CreateElementRequest _request { get;}
        public CreateElementCommand(CreateElementRequest request)
        {
            _request = request;
        }
    }
}
