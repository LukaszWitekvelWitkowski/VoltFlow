using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Client.DTOs;
using VoltFlow.Service.Core.Models.Client.Requests;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Core.Abstractions.Repositories
{
    public interface IClientRepository
    {
        Task AddAsync(Client newClient, CancellationToken ct);
        Task<PagedResultDTO<ClientDTO>> GetAllClientsAsync(string? email, int page, int size,CancellationToken ct);
        Task<ClientDTO?> GetByEmailAsync(string email, CancellationToken ct);
        Task<ClientDTO> UpdateCleintAsync(ClientRequest request, CancellationToken ct);
    }
}
