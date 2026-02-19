using MediatR;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Auth.Request;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Commands.Auth
{
    public record ConfirmEmailCommand : IRequest<ServiceResponse<Result>>
    {
        public ConfirmEmailCommand(ConfirmEmailRequest request)
        {
            this.request = request;
        }

        public ConfirmEmailRequest request { get; }
    }
}
