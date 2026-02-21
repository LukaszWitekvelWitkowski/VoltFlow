using MediatR;
using VoltFlow.Service.Core.Enums;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Queries.Client
{
    public class GetClientStatusQuery  : IRequest<ServiceResponse<StatusClient>>
    {
        public string Email { get; set; }
        public GetClientStatusQuery(string email)
        {
            Email = email;
        }
    }
}
