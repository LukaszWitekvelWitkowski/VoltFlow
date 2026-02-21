using MediatR;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Client.Requests;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Commands.Client
{
    public class UpdateClientCommand : IRequest<ServiceResponse<Result>>
    {
        public UpdateClientCommand(ClientRequest request)
        {
            this.request = request;
        }

        public ClientRequest request { get; }

    }
}
