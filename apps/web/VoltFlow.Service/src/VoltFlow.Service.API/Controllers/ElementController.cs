using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoltFlow.Service.API.Base;
using VoltFlow.Service.Application.Queries.Element;

namespace VoltFlow.Service.API.Controllers
{
    [ApiController]
    public class ElementController : ApiControllerBase
    {
        public ElementController(IMediator mediator) : base(mediator)
        {
        }

        #region GET


        [HttpGet("")]
        public async Task<IActionResult> GetElementsAsync() => await HandlerAsync(new GetElementsQuery());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetElementByIdAsync(int id) => await HandlerAsync(new GetElementByIdQuery(id));

        [HttpGet("search/{Name}")]
        public async Task<IActionResult> GetElementByNameAsync(string name) => await HandlerAsync(new GetElementByNameQuery(name));

        #endregion
        #region POST

        #endregion
        #region PUT

        #endregion
        #region DELETE

        #endregion

    }
}
