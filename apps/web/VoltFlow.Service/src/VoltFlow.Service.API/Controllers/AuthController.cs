using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoltFlow.Service.API.Base;
using VoltFlow.Service.Application.Commands.Auth;
using VoltFlow.Service.Application.Queries.Catalog;
using VoltFlow.Service.Core.Models.Requests;

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
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command) => await HandlerAsync(command);

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command) => await HandlerAsync(command);

        #endregion
        #region PUT

        #endregion
        #region DELETE

        #endregion
    }
}
