using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.ClientAddress.DTOs;
using VoltFlow.Service.Core.Models.ClientAddress.Request;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Core.Abstractions.Repositories
{
    public interface IClientAdressRepository
    {
        Task<ServiceResponse<ClientAddressDTO>> AddOrUpdateAddressAsync(ClientAddressRequest addressDto, CancellationToken ct);
        Task<ServiceResponse<ClientAddressCacheDTO>> GetClientAddressFromCacheAsync(int clientId, CancellationToken ct);
 
    }
}
