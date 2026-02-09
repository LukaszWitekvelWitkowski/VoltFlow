using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using VoltFlow.Service.Core.Exceptions;

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
            // DODAJEMY TO:
            catch (ConflictException ex)
            {
                // To zwróci status 409
                return Conflict(ex.Message);
            }
            catch (ValidationEntityException ex)
            {
                // To zostanie jako 400
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                // To zwróci status 404
                return NotFound(ex.Message);
            }
            // OSTATNIA DESKA RATUNKU:
            catch (Exception ex)
            {
                // Prawdziwe, nieoczekiwane błędy powinny zwracać 500
                return StatusCode(500, "Wystąpił nieoczekiwany błąd serwera.");
            }
        }
    }
}
