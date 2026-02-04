using VoltFlow.Service.Core.Models;
using VoltFlow.Service.Core.Models.Element;

namespace VoltFlow.Service.Core.Abstractions.Repositories
{
    public interface IElementRepository
    {
        Task<ServiceResponse<ElementsDTO>> GetElementsQuery();
        Task<ServiceResponse<ElementDTO>> GetElementByIdQuery(int id);
        Task<ServiceResponse<ElementDTO>> GetElementByNameQuery(string name);

    }
}
