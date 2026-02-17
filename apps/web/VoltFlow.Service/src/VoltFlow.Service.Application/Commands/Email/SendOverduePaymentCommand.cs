using MediatR;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Commands.Email
{
    public record SendOverduePaymentCommand(
        int ClientId,
        string CustomerEmail,
        string CustomerName,
        decimal Amount,
        DateTime DueDate
    ) : IRequest<ServiceResponse<Result>>;
}
