using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;

namespace VoltFlow.Service.Core.Abstractions.Repositories
{
    public interface IElementGroupRepository
    {
        Task<ServiceResponse<ElementGroupsDTO>> GetElementGroupsQuery();
        Task<ServiceResponse<ElementGroupDTO>> GetElementGroupByIdQuery(int id);
        Task<ServiceResponse<PagedResultDTO<ElementGroupDTO>>> GetElementGroupSearchQuery(string? name, int page, int size);
    }
}
