using MediatR;
using VoltFlow.Service.Application.Queries.Client;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Enums;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Infrastructure.Handlers.Client
{
    public class GetClientStatusHandler : IRequestHandler<GetClientStatusQuery, ServiceResponse<ClientStatus>>
    {
        private readonly IClientRepository _clientRepository;
        public GetClientStatusHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
        public async Task<ServiceResponse<ClientStatus>> Handle(GetClientStatusQuery request, CancellationToken ct)
        {
            var client = await _clientRepository.GetByEmailAsync(request.Email, ct);
            if (client == null)
            {
                return ServiceResponse<ClientStatus>.Failure("Client not found.");
            }
            return ServiceResponse<ClientStatus>.Success(client.StatusClient);
        }
    }
}
