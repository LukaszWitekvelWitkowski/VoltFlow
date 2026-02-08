using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Element.DTOs;

namespace VoltFlow.Service.Core.Abstractions.Repositories
{
    public interface IElementRepository
    {
        Task<ServiceResponse<ElementsDTO>> GetElementsQuery();
        Task<ServiceResponse<ElemntGroupDTO>> GetElementByIdQuery(int id);
        Task<ServiceResponse<PagedResultDTO<ElemntGroupDTO>>> GetElementsPagedByNameQuery(string? name, int page, int size);

    }
}
