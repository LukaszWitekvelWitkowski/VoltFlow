using MediatR;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Element.DTOs;
using VoltFlow.Service.Core.Models.Element.Request;

namespace VoltFlow.Service.Application.Commands.Element
{
    public class UpdateElementCommand : IRequest<ServiceResponse<ElementDTO>>
    {
        public UpdateElementCommand(UpdateElementRequest request)
        {
            Request = request;
        }

        public UpdateElementRequest Request { get;}   
    }
}
