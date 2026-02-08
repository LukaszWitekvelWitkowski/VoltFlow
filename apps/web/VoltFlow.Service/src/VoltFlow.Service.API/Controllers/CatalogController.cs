using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoltFlow.Service.API.Base;
using VoltFlow.Service.Application.Queries.Catalog;
using VoltFlow.Service.Core.Models.Requests;

namespace VoltFlow.Service.API.Controllers
{
    public class CatalogController : ApiControllerBase
    {
        public CatalogController(IMediator mediator) : base(mediator)
        {
        }


        #region GET

        [HttpGet("search")]
        public async Task<IActionResult> GetCatalogSearchAsync([FromQuery] CatalogSearchRequest request) => await HandlerAsync(new GetCatalogSearchQuery(request));


        #endregion
        #region POST

        #endregion
        #region PUT

        #endregion
        #region DELETE

        #endregion
    }
}
