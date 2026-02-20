using MediatR;
using VoltFlow.Service.Application.Commands.Client;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Infrastructure.Handlers.Client
{
    public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, ServiceResponse<Result>>
    {
        private readonly IClientService _clientService;


        public UpdateClientCommandHandler(IClientService clientService)
        {
            _clientService = clientService;
        }

        public async Task<ServiceResponse<Result>> Handle(UpdateClientCommand command, CancellationToken cancellationToken)
        {
            var clientUpdateResponse = await _clientService.UpdateClientProfileAsync(command.request, cancellationToken);

            if (!clientUpdateResponse._IsSuccess)
            {
                return ServiceResponse<Result>.Failure(clientUpdateResponse._Message);
            }

            return ServiceResponse<Result>.Success(Result.isSucces());
        }
    }
}