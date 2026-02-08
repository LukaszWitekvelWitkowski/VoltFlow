using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoltFlow.Service.API.Base;
using VoltFlow.Service.Application.Commands.Category;
using VoltFlow.Service.Application.Queries.Category;
using VoltFlow.Service.Core.Models.Category;
using VoltFlow.Service.Core.Models.Category.Request;
using VoltFlow.Service.Core.Models.Requests;

namespace VoltFlow.Service.API.Controllers
{
    [ApiController]
    public class CategoryController : ApiControllerBase
    {
        public CategoryController(IMediator mediator) : base(mediator)
        {
        }

        #region GET

        [HttpGet("")]
        public async Task<IActionResult> GetCategoriesAsync() => await HandlerAsync(new GetCategoriesQuery());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryByIdAsync(int id) => await HandlerAsync(new GetCategoryByIdQuery(id));

        [HttpGet("search")]
        public async Task<IActionResult> GetCategoryByNameAsync([FromQuery] SearchRequest request) => await HandlerAsync(new GetCategorySearchQuery(request.Name, request.Number, request.Size));


        #endregion
        #region POST

        #endregion

        [HttpPost("")]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryRequest request) => await HandlerAsync(new CreateCategoryCommand(request.Name));

        #region PUT

        #endregion
        #region DELETE

        #endregion
    }
}
