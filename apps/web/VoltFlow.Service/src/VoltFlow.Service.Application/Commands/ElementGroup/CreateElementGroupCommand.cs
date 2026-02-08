using MediatR;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;
using VoltFlow.Service.Core.Models.ElementGroup.Request;

namespace VoltFlow.Service.Application.Commands.ElementGroup
{
    public class CreateElementGroupCommand :IRequest<ServiceResponse<ElementGroupDTO>>
    {
        public CreateElementGroupRequest _request { get; }

        public CreateElementGroupCommand(CreateElementGroupRequest request)
        {
            _request = request;
        }
    }
}
