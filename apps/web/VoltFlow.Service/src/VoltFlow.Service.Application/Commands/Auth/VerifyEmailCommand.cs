using MediatR;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Auth.Request;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Commands.Auth
{
    public class VerifyEmailCommand : IRequest<ServiceResponse<Result>>
    {
        public VerifyEmailCommand(VerifyEmailRequest request)
        {
            this.request = request;
        }
        public VerifyEmailRequest request { get; }
    }
}
