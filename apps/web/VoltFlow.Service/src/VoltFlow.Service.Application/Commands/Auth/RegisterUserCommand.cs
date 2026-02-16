using MediatR;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Commands.Auth
{
    public class RegisterUserCommand : IRequest<ServiceResponse<Result>>
    {
        public RegisterUserCommand(RegisterUserRequest registerUserRequest)
        {
            RegisterUserRequest = registerUserRequest;
            Role = 1;

        }

        public RegisterUserCommand(RegisterUserRequest registerUserRequest, int role) 
        {
            RegisterUserRequest = registerUserRequest;
            Role = role;
        }

        public RegisterUserRequest RegisterUserRequest { get; }

        public int Role { get; }
    }
}
