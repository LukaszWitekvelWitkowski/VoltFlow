using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace VoltFlow.Service.API.Base
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected readonly IMediator _mediator;

        protected ApiControllerBase(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected async Task<IActionResult> HandlerAsync<T>(IRequest<T> query)
        {
            try
            {
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }
    }
}
