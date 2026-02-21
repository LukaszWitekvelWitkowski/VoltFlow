using MediatR;
using VoltFlow.Service.Application.Queries.ClientAddress;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models.ClientAddress.DTOs;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Infrastructure.Handlers.ClientAddress
{
    public class GetClientAddressByClientIdHandler : IRequestHandler<GetClientAddressByClientIdQuery, ServiceResponse<ClientAddressCacheDTO>>
    {
        private readonly IClientAdressRepository _clientAddressRepository;

        public GetClientAddressByClientIdHandler(IClientAdressRepository clientAddressRepository)
        {
            _clientAddressRepository = clientAddressRepository;
        }

        public async Task<ServiceResponse<ClientAddressCacheDTO>> Handle(GetClientAddressByClientIdQuery request, CancellationToken cancellationToken)
        {
           return await _clientAddressRepository.GetClientAddressFromCacheAsync(request.IdClient, cancellationToken);

        }
    }
}
