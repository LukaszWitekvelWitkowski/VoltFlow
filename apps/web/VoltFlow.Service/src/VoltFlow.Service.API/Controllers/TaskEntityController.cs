using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoltFlow.Service.API.Base;
using VoltFlow.Service.Application.Commands.TaskEntity;
using VoltFlow.Service.Application.Queries.TaskEntity;
using VoltFlow.Service.Core.Models.Requests;
using VoltFlow.Service.Core.Models.TaskEntity.Request;

namespace VoltFlow.Service.API.Controllers
{
    public class TaskEntityController :ApiControllerBase
    {
        public TaskEntityController(IMediator mediator) : base(mediator)
        {
        }

        #region GET
        [HttpGet("")]
        public async Task<IActionResult> GetTaskEntityAsync() => await HandlerAsync(new GetTaskEntitiesQuery());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskEntityByIdAsync([FromRoute] int id) => await HandlerAsync(new GetTaskEntityByIdQuery(id));

        [HttpGet("search")]
        public async Task<IActionResult> GetTaskEntitySearchAsync([FromQuery] SearchRequest request) => await HandlerAsync(new GetTaskEntitySearchQuery(request.Name, request.Number, request.Size));

        #endregion

        #region POST
        [HttpPost("")]
        public async Task<IActionResult> CreateTaskEntityAsync([FromBody] CreateTaskEntityRequest request) => await HandlerAsync(new CreateTaskEntityCommand(request));
        #endregion

        #region PUT
        [HttpPut("")]
        public async Task<IActionResult> UpdateElementAsync([FromBody] UpdateTaskEntityRequest request) => await HandlerAsync(new UpdateTaskEntityCommand(request));
        #endregion
    }
}
