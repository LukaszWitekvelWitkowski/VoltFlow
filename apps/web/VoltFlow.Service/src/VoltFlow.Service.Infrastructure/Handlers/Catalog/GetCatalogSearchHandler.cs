using MediatR;
using VoltFlow.Service.Application.Queries.Catalog;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models.Catalog.DTOs;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Validators;

namespace VoltFlow.Service.Infrastructure.Handlers.Catalog
{
    public class GetCatalogSearchHandler : IRequestHandler<GetCatalogSearchQuery, ServiceResponse<PagedResultDTO<ElementTreeDTO>>>
    {
        private readonly ICatalogRepository _catalogRepository;

        public GetCatalogSearchHandler(ICatalogRepository catalogRepository)
        {
            _catalogRepository = catalogRepository;
        }

        public async Task<ServiceResponse<PagedResultDTO<ElementTreeDTO>>> Handle(GetCatalogSearchQuery request, CancellationToken cancellationToken)
        {
            return await ResponseValidator.ExecuteAsync(async () => await _catalogRepository.GetCatalogSearchQuery(request._request), "Catalog");
        }
    }
}
