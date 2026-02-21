using MediatR;
using VoltFlow.Service.Core.Models.ClientAddress.DTOs;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Queries.ClientAddress
{
    public class GetClientAddressByClientIdQuery : IRequest<ServiceResponse<ClientAddressCacheDTO>>
    {
        public int IdClient { get; set; }
        public GetClientAddressByClientIdQuery(int idClient)
        {
            IdClient = idClient;
        }
    }
}
