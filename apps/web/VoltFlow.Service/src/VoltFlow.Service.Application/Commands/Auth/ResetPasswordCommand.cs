using MediatR;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Commands.Auth
{
    public class ResetPasswordCommand : IRequest<ServiceResponse<Result>>
    {
        public ResetPasswordCommand(string email)
        {
            Email = email;
        }

        public string Email { get; }


    }
}
