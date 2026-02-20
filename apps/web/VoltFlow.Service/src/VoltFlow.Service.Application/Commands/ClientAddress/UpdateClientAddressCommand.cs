using MediatR;
using VoltFlow.Service.Core.Models.ClientAddress.DTOs;
using VoltFlow.Service.Core.Models.ClientAddress.Request;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Commands.ClientAddress
{
    public class UpdateClientAddressCommand : IRequest<ServiceResponse<ClientAddressDTO>>
    {
        public UpdateClientAddressCommand(ClientAddressRequest request)
        {
            this.request = request;
        }

        public ClientAddressRequest request { get; }

    }
}
