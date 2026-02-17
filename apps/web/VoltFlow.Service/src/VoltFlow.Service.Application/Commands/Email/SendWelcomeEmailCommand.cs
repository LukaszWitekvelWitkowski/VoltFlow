using MediatR;

namespace VoltFlow.Service.Application.Commands.Email
{
    public record SendWelcomeEmailCommand(string CustomerEmail, string CustomerName) : IRequest<bool>
    {
    }
}
