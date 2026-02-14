using MediatR;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Commands.Auth
{
    public class RegisterUserCommand : IRequest<ServiceResponse<Result>>
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public required string Login { get; set; }

    }
}
