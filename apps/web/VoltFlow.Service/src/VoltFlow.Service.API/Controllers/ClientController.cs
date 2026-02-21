using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoltFlow.Service.API.Base;
using VoltFlow.Service.Application.Commands.Client;
using VoltFlow.Service.Application.Queries.Client;
using VoltFlow.Service.Core.Models.Client.Requests;

namespace VoltFlow.Service.API.Controllers
{
    public class ClientController : ApiControllerBase
    {
        public ClientController(IMediator mediator) : base(mediator)
        {
        }

        #region GET
        [HttpGet("search")]
        public async Task<IActionResult> GetClientsPagedByEmailAsync([FromQuery] string email, [FromQuery] int number, [FromQuery] int size) => await HandlerAsync(new GetClientSearchQuery(email, number, size));

        [HttpGet("Status")]
        public async Task<IActionResult> GetClientsPagedByStatusAsync([FromQuery] string email ) => await HandlerAsync(new GetClientStatusQuery(email));
        #endregion
        #region POST
        #endregion
        #region PUT
        [HttpPut("")]
        public async Task<IActionResult> UpdateClientAsync([FromBody] ClientRequest request) => await HandlerAsync(new UpdateClientCommand(request));
        #endregion
        #region DELETE
        #endregion
    }
}
