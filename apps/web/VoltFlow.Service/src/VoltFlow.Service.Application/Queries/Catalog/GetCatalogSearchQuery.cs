using MediatR;
using VoltFlow.Service.Core.Models.Catalog.DTOs;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Requests;

namespace VoltFlow.Service.Application.Queries.Catalog
{
    public class GetCatalogSearchQuery : PaginationParams, IRequest<ServiceResponse<PagedResultDTO<ElementTreeDTO>>>
    {
        public GetCatalogSearchQuery(CatalogSearchRequest request)
        {
            _request = request;
        }


        public CatalogSearchRequest _request { get; }

    }
}
