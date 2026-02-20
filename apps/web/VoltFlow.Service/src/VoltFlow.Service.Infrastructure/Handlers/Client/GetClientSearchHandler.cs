using MediatR;
using VoltFlow.Service.Application.Queries.Client;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models.Client.DTOs;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Infrastructure.Handlers.Client
{
    public class GetClientSearchHandler : IRequestHandler<GetClientSearchQuery, ServiceResponse<PagedResultDTO<ClientDTO>>>
    {
        private readonly IClientRepository _clientRepository;
        public GetClientSearchHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
        public async Task<ServiceResponse<PagedResultDTO<ClientDTO>>> Handle(GetClientSearchQuery request, CancellationToken ct)
        {
            return await _clientRepository.GetAllClientsAsync(request.Email, request.PageNumber, request.PageSize, ct);
        }
    }
}
