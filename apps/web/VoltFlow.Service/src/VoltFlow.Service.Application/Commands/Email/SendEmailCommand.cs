using MediatR;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Commands.Email
{
    public record SendEmailCommand(
        int ClientId,
        string CustomerEmail,
        string CustomerName,
        DateTime DueDate
    ) : IRequest<ServiceResponse<Result>>;
}
