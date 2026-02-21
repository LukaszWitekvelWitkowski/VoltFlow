using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoltFlow.Service.API.Base;
using VoltFlow.Service.Application.Commands.ClientAddress;
using VoltFlow.Service.Application.Queries.ClientAddress;
using VoltFlow.Service.Core.Models.ClientAddress.Request;

namespace VoltFlow.Service.API.Controllers
{
    public class ClientAddressCnotroller : ApiControllerBase
    {
        public ClientAddressCnotroller(IMediator mediator) : base(mediator)
        {
        }

        #region GET
        [HttpGet("")]
        public async Task<IActionResult> GetClientAddressAsync([FromQuery] int ClietId) => await HandlerAsync(new GetClientAddressByClientIdQuery(ClietId));
        #endregion
        #region POST
        #endregion
        #region PUT
        [HttpPut("")]
        public async Task<IActionResult> UpdateClientAddressAsync([FromBody] ClientAddressRequest  request) => await HandlerAsync(new UpdateClientAddressCommand(request));
        #endregion
        #region DELETE
        #endregion
    }
}
