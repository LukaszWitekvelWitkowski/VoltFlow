using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Client.DTOs;
using VoltFlow.Service.Core.Models.Client.Requests;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Core.Abstractions.Services
{
    public interface IClientService
    {
        Task<int> CreateClientFromUserAsync(User user, CancellationToken ct);
        Task<ServiceResponse<ClientDTO>> UpdateClientProfileAsync(ClientRequest command, CancellationToken ct);
        Task<ServiceResponse<PagedResultDTO<ClientDTO>>> GetAllClientsAsync(string? email, int page, int size, CancellationToken ct);
    }
}
