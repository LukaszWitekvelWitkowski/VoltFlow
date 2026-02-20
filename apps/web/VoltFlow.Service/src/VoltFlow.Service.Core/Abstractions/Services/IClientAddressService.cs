using VoltFlow.Service.Core.Models.ClientAddress.DTOs;
using VoltFlow.Service.Core.Models.ClientAddress.Request;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Core.Abstractions.Services
{
    public interface IClientAddressService
    {
        Task<ServiceResponse<ClientAddressDTO>> AddOrUpdateAddressAsync(ClientAddressRequest addressDto, CancellationToken ct);
    }
}
