using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoltFlow.Service.API.Base;
using VoltFlow.Service.Application.Queries.Category;
using VoltFlow.Service.Core.Models.Category;

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

        [HttpGet("search/{Name}")]
        public async Task<IActionResult> GetCategoryByNameAsync(string name) => await HandlerAsync(new GetCategoryByNameQuery(name));


        #endregion
        #region POST

        #endregion
        #region PUT

        #endregion
        #region DELETE

        #endregion
    }
}
