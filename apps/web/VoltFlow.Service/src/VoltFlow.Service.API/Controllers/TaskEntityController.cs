using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoltFlow.Service.API.Base;
using VoltFlow.Service.Application.Queries.TaskEntity;
using VoltFlow.Service.Core.Models.Requests;

namespace VoltFlow.Service.API.Controllers
{
    public class TaskEntityController :ApiControllerBase
    {
        public TaskEntityController(IMediator mediator) : base(mediator)
        {
        }



        #region GET


        [HttpGet("")]
        public async Task<IActionResult> GetElementsAsync() => await HandlerAsync(new GetTaskEntitiesQuery());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetElementByIdAsync([FromRoute] int id) => await HandlerAsync(new GetTaskEntityByIdQuery(id));

        [HttpGet("search")]
        public async Task<IActionResult> GetElementsPagedByNameAsync([FromQuery] SearchRequest request) => await HandlerAsync(new GetTaskEntitySearchQuery(request.Name, request.Number, request.Size));

        #endregion
        #region POST

        #endregion
        #region PUT

        #endregion
        #region DELETE

        #endregion
    }
}
