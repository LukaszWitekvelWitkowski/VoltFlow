using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoltFlow.Service.API.Base;
using VoltFlow.Service.Application.Commands.Email;

namespace VoltFlow.Service.API.Controllers
{
    public class EmailController : ApiControllerBase
    {
        public EmailController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost("send-test-email")]
        public async Task<IActionResult> SendTestEmail([FromBody] SendOverduePaymentCommand send) => await HandlerAsync(send);
    }
}
