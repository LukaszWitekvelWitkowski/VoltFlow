using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoltFlow.Service.API.Base;
using VoltFlow.Service.Application.Commands.Auth;
using VoltFlow.Service.Application.Queries.Auth;
using VoltFlow.Service.Core.Models.Auth;

namespace VoltFlow.Service.API.Controllers
{
    public class AuthController : ApiControllerBase
    {
        public AuthController(IMediator mediator) : base(mediator)
        {
        }

        #region GET


        #endregion
        #region POST
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest command) => await HandlerAsync(new RegisterUserCommand(command));

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command) => await HandlerAsync(command);

        [HttpPost("register-employee")]
        public async Task<IActionResult> RegisterEmployee([FromBody] RegisterUserRequest command, int role) => await HandlerAsync(new RegisterUserCommand(command, role));

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command) => await HandlerAsync(command);

        [HttpPost("confirm-reset-password")]
        public async Task<IActionResult> ConfirmResetPassword([FromBody] ConfirmResetPasswordRequest request) => await HandlerAsync(new ConfirmResetPasswordCommand(request));
        #endregion
        #region PUT

        #endregion
        #region DELETE

        #endregion
    }
}
