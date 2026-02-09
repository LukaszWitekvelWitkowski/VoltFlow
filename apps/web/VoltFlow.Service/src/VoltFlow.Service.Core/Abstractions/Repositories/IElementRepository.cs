using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Element.DTOs;
using VoltFlow.Service.Core.Models.Element.Request;

namespace VoltFlow.Service.Core.Abstractions.Repositories
{
    public interface IElementRepository
    {
        Task<ServiceResponse<ElementsDTO>> GetElementsQuery();
        Task<ServiceResponse<ElementDTO>> GetElementByIdQuery(int id);
        Task<ServiceResponse<PagedResultDTO<ElementDTO>>> GetElementsPagedByNameQuery(string? name, int page, int size);
        Task<ServiceResponse<ElementDTO>> AddElement(CreateElementRequest name);
        Task<ServiceResponse<ElementDTO>> UpdateElement(UpdateElementRequest request);
        Task<bool> IsExists(string name, int? id = null);
    }
}
