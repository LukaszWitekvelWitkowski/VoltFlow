using MediatR;
using VoltFlow.Service.Application.Queries.Auth;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Commands.Auth
{
    public class ConfirmResetPasswordCommand : IRequest<ServiceResponse<Result>>
    {
        public ConfirmResetPasswordRequest confirmResetPasswordRequest { get;}

        public ConfirmResetPasswordCommand(ConfirmResetPasswordRequest confirmResetPasswordRequest)
        {
            this.confirmResetPasswordRequest = confirmResetPasswordRequest;
        }
    }
}
