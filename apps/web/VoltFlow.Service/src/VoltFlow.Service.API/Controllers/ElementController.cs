using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoltFlow.Service.API.Base;
using VoltFlow.Service.Application.Commands.Category;
using VoltFlow.Service.Application.Commands.Element;
using VoltFlow.Service.Application.Queries.Element;
using VoltFlow.Service.Core.Models.Category.Request;
using VoltFlow.Service.Core.Models.Element.Request;
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
        public async Task<IActionResult> GetElementsPagedByNameAsync([FromQuery] SearchRequest request) => await HandlerAsync(new GetElementSearchQuery(request.Name, request.Number, request.Size));

        #endregion
        #region POST
    
        [HttpPost("")]
        public async Task<IActionResult> CreateElementAsync([FromBody] CreateElementRequest request) => await HandlerAsync(new CreateElementCommand(request));

        #endregion


        #region PUT
   
        [HttpPut("")]
        public async Task<IActionResult> UpdateElementAsync([FromBody] UpdateElementRequest request) => await HandlerAsync(new UpdateElementCommand(request));
        #endregion

    }
}
