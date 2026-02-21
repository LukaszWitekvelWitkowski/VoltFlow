using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Element.DTOs;
using VoltFlow.Service.Core.Models.Element.Request;

namespace VoltFlow.Service.Core.Abstractions.Repositories
{
    public interface IElementRepository
    {
        Task<ElementCacheDTO> GetElementsQuery();
        Task<ElementDTO?> GetElementByIdQuery(int id);
        Task<PagedResultDTO<ElementDTO>> GetElementsPagedByNameQuery(string? name, int page, int size);
        Task<ElementDTO> AddElement(CreateElementRequest name);
        Task<ElementDTO> UpdateElement(UpdateElementRequest request);
        Task<bool> IsExists(string name, int? id = null);
    }
}
