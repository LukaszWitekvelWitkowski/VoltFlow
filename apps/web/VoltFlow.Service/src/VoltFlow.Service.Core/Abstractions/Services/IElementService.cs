using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Element.DTOs;
using VoltFlow.Service.Core.Models.Element.Request;

namespace VoltFlow.Service.Core.Abstractions.Services
{
    public interface IElementService
    {
        Task<ServiceResponse<ElementDTO>> CreateElement(string name);

        Task<ServiceResponse<ElementDTO>> UpdateElement(UpdateElementRequest request);
    }
}
