using MediatR;
using VoltFlow.Service.Application.Commands.Email;
using VoltFlow.Service.Core.Abstractions.Services;

namespace VoltFlow.Service.Infrastructure.Handlers.Email
{
    public class SendWelcomeEmailHandler : IRequestHandler<SendWelcomeEmailCommand, bool>
    {
        private readonly IEmailService _emailService;

        public SendWelcomeEmailHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<bool> Handle(SendWelcomeEmailCommand request, CancellationToken cancellationToken)
        {
            // Tutaj możesz przygotować konkretny szablon CRM
            var subject = $"Witaj w naszym CRM, {request.CustomerName}!";
            var body = $"<h1>Cześć {request.CustomerName}</h1><p>Twoje konto jest już aktywne.</p>";

            // Delegujemy techniczną wysyłkę do serwisu
            return await _emailService.SendEmailAsync(request.CustomerEmail, subject, body, cancellationToken);
        }
    }
}
