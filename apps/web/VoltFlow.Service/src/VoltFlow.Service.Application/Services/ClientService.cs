using Microsoft.Extensions.Logging;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Enums;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Client.DTOs;
using VoltFlow.Service.Core.Models.Client.Requests;
using VoltFlow.Service.Core.Models.ClientAddress.DTOs;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IClientAdressRepository _addressRepository;
        private readonly ILogger<ClientService> _logger;

        public ClientService(IClientRepository clientRepository, ILogger<ClientService> logger, IClientAdressRepository addressRepository)
        {
            _clientRepository = clientRepository;
            _logger = logger;
            _addressRepository = addressRepository;
        }

        public async Task<ServiceResponse<Result>> AddOrUpdateAddressAsync(int clientId, ClientAddressDTO addressDto, CancellationToken ct)
        {
            return _addressRepository.AddOrUpdateAddressAsync(clientId, addressDto, ct);
        }

        public async Task<ServiceResponse<int>> CreateClientFromUserAsync(User user, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(user.Email))
            {
                _logger.LogWarning("User {UserId} does not have an email, cannot create client record.", user.Id);
                return ServiceResponse<int>.Failure("User must have an email to create a client record.");
            }

            var newClient = new Client
            {
                IdClient = user.Id,
                Email = user.Email,
                CreatedAt = DateTime.UtcNow,
                statusClient = StatusClient.Active
            };

            try
            {
                await _clientRepository.AddAsync(newClient, ct);
                return ServiceResponse<int>.Success(newClient.IdClient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating client for user {UserId}", user.Id);
                return ServiceResponse<int>.Failure("Failed to create client record.");
            }
        }

        public async Task<ServiceResponse<PagedResultDTO<ClientDTO>>> GetAllClientsAsync(string? email, int page, int size, CancellationToken ct)
        {
            return await _clientRepository.GetAllClientsAsync(email, page, size,ct);
        }

        public Task<ServiceResponse<ClientAddressDTO>> GetClientAddressAsync(int clientId, CancellationToken ct)
        {
            return _addressRepository.GetClientAddressAsync(clientId, ct);
        }

        public async Task<ServiceResponse<ClientDTO>> UpdateClientProfileAsync(ClientRequest request, CancellationToken ct)
        {
            return await _clientRepository.UpdateCleintAsync(request, ct);
        }
    }
}
