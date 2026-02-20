using MediatR;
using VoltFlow.Service.Application.Commands.ClientAddress;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models.ClientAddress.DTOs;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Infrastructure.Handlers.ClientAddress
{
    public class UpdateClientAddressHandler : IRequestHandler<UpdateClientAddressCommand, ServiceResponse<ClientAddressDTO>>
    {
        private readonly IClientAdressRepository _addressRepository;
        public UpdateClientAddressHandler(IClientAdressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }
        public async Task<ServiceResponse<ClientAddressDTO>> Handle(UpdateClientAddressCommand request, CancellationToken ct)
        {
            var result = await _addressRepository.AddOrUpdateAddressAsync(request.request, ct);
            if (!result._IsSuccess)
            {
                return ServiceResponse<ClientAddressDTO>.Failure(result._Message);
            }
            return ServiceResponse<ClientAddressDTO>.Success(result._Data);
        }

    }
}
