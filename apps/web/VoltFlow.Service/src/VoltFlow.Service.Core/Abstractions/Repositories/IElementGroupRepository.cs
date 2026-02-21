using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;
using VoltFlow.Service.Core.Models.ElementGroup.Request;

namespace VoltFlow.Service.Core.Abstractions.Repositories
{
    public interface IElementGroupRepository
    {
        Task<ElementGroupCacheDTO> GetElementGroupsQuery();
        Task<ElementGroupDTO?> GetElementGroupByIdQuery(int id);
        Task<PagedResultDTO<ElementGroupDTO>> GetElementGroupSearchQuery(string? name, int page, int size);
        Task<ElementGroupDTO> AddElementGroup(CreateElementGroupRequest request);
        Task<ElementGroupDTO> UpdateElementGroup(UpdateElementGroupRequest request);
        Task<bool> IsExists(string name, int? idElementGroup = null);
    }
}
