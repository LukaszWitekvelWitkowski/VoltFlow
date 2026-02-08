using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;
using VoltFlow.Service.Core.Models.ElementGroup.Request;

namespace VoltFlow.Service.Core.Abstractions.Services
{
    public interface IElementGroupService
    {
        Task<ServiceResponse<ElementGroupDTO>> CreateElementGroup(CreateElementGroupRequest name);

        Task<ServiceResponse<ElementGroupDTO>> UpdateElementGroup(UpdateElementGroupRequest request);
    }
}
