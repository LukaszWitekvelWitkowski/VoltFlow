using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoltFlow.Service.API.Base;
using VoltFlow.Service.Application.Queries.Element;
using VoltFlow.Service.Core.Models.Requests;

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
        public async Task<IActionResult> GetElementByIdAsync([FromRoute] int id) => await HandlerAsync(new GetElementByIdQuery(id));

        [HttpGet("search")]
        public async Task<IActionResult> GetElementsPagedByNameAsync([FromQuery] SearchRequest request) => await HandlerAsync(new GetElementByNameQuery(request.Name, request.Number, request.Size));

        #endregion
        #region POST

        #endregion
        #region PUT

        #endregion
        #region DELETE

        #endregion

    }
}
