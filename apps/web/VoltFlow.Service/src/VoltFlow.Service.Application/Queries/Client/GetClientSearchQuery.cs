using MediatR;
using VoltFlow.Service.Core.Models.Client.DTOs;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Queries.Client
{
    public class GetClientSearchQuery : PaginationParams , IRequest<ServiceResponse<PagedResultDTO<ClientDTO>>>
    {
        public string? Email { get; set; }

        public GetClientSearchQuery(string? email, int number, int size)
        {
            Email = email;
            PageNumber = number;
            PageSize = size;
        }

    }
}
