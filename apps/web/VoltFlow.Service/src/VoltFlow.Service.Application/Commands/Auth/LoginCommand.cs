using MediatR;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Commands.Auth
{
    public class LoginCommand : IRequest<ServiceResponse<TokenResponse>>
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
