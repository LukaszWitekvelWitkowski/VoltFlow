using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoltFlow.Service.API.Base;
using VoltFlow.Service.Application.Queries.Role;

namespace VoltFlow.Service.API.Controllers
{
    public class RoleController : ApiControllerBase
    {
        public RoleController(IMediator mediator) : base(mediator)
        {
        }

        #region GET
        [HttpGet("")]
        public async Task<IActionResult> GetRolesAsync() => await HandlerAsync(new GetRolesQuery());

        #endregion
        #region POST

        #endregion
        #region PUT

        #endregion
        #region DELETE

        #endregion
    }
}
