using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.ClientAddress.DTOs;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Core.Abstractions.Repositories
{
    public interface IClientAdressRepository
    {
        ServiceResponse<Result> AddOrUpdateAddressAsync(int clientId, ClientAddressDTO addressDto, CancellationToken ct);
        Task<ServiceResponse<ClientAddressDTO>> GetClientAddressAsync(int clientId, CancellationToken ct);
    }
}
