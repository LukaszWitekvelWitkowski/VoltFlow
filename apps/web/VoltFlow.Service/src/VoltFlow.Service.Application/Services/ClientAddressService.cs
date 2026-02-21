using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Models.ClientAddress.DTOs;
using VoltFlow.Service.Core.Models.ClientAddress.Request;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Services
{
    public class ClientAddressService : IClientAddressService
    {
        private readonly IClientAdressRepository _clientAddressRepository;

        public ClientAddressService(IClientAdressRepository clientAddressService)
        {
            _clientAddressRepository = clientAddressService;
        }

        public async Task<ServiceResponse<ClientAddressDTO>> AddOrUpdateAddressAsync(ClientAddressRequest request, CancellationToken ct)
        {
           return await _clientAddressRepository.AddOrUpdateAddressAsync(request, ct);
        }
    }
}
