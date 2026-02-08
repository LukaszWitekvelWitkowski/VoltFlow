using VoltFlow.Service.Core.Models.Catalog.DTOs;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Requests;

namespace VoltFlow.Service.Core.Abstractions.Repositories
{
    public interface ICatalogRepository
    {
        public Task<ServiceResponse<PagedResultDTO<ElementTreeDTO>>> GetCatalogSearchQuery(CatalogSearchRequest request);
    }
}
