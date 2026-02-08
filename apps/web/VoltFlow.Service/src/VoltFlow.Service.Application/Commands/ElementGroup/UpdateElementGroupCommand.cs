using MediatR;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;
using VoltFlow.Service.Core.Models.ElementGroup.Request;

namespace VoltFlow.Service.Application.Commands.ElementGroup
{
    public class UpdateElementGroupCommand : IRequest<ServiceResponse<ElementGroupDTO>>
    {
        public UpdateElementGroupRequest _request { get; }

        public UpdateElementGroupCommand(UpdateElementGroupRequest request)
        {
            _request = request;
        }
    }
}
