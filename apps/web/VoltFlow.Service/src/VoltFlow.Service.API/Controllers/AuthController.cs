using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoltFlow.Service.API.Base;
using VoltFlow.Service.Application.Commands.Auth;
using VoltFlow.Service.Application.Queries.Auth;
using VoltFlow.Service.Core.Models.Auth.Request;

namespace VoltFlow.Service.API.Controllers
{
    public class AuthController : ApiControllerBase
    {
        public AuthController(IMediator mediator) : base(mediator)
        {
        }

        #region GET
        // GET endpoints can be added here (e.g., for checking registration status)
        #endregion

        #region POST

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest command)
            => await HandlerAsync(new RegisterUserCommand(command));

        /// <summary>
        /// Authenticates a user and returns security tokens.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
            => await HandlerAsync(command);

        /// <summary>
        /// Registers a new employee with a specific organizational role.
        /// </summary>
        [HttpPost("register-employee")]
        public async Task<IActionResult> RegisterEmployee([FromBody] RegisterUserRequest command, int role)
            => await HandlerAsync(new RegisterUserCommand(command, role));

        /// <summary>
        /// Initiates the password reset process by sending an email to the user.
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
            => await HandlerAsync(command);

        /// <summary>
        /// Finalizes the password reset process using a verification token.
        /// </summary>
        [HttpPost("confirm-reset-password")]
        public async Task<IActionResult> ConfirmResetPassword([FromBody] ConfirmResetPasswordRequest request)
            => await HandlerAsync(new ConfirmResetPasswordCommand(request));

        /// <summary>
        /// Triggers the email confirmation process (sends a verification link).
        /// </summary>
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
            => await HandlerAsync(new ConfirmEmailCommand(request));


        /// <summary>
        /// Verifies the user's email address using the token received in the email.
        /// </summary>
        [HttpPost("verify-email")] // Fixed typo: veryfy -> verify
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
            => await HandlerAsync(new VerifyEmailCommand(request));

        #endregion

        #region PUT
        // PUT endpoints for updating user profiles or passwords
        #endregion

        #region DELETE
        // DELETE endpoints for account removal
        #endregion
    }
}
