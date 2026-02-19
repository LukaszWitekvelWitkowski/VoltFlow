using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Polly;
using Polly.Retry;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Models.email;

namespace VoltFlow.Service.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _settings;
        private readonly ILogger<EmailService> _logger;
        private readonly AsyncRetryPolicy _retryPolicy; 

        public EmailService(IOptions<SmtpSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;

            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning("Próba {RetryCount} nieudana. Retrying in {TimeSpan}. Błąd: {Message}",
                            retryCount, timeSpan, exception.Message);
                    });
        }
        public async Task<bool> SendEmailAsync(string to, string subject, string body, CancellationToken ct = default)
        {
            try
            {
                return await _retryPolicy.ExecuteAsync(async () =>
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("System CRM", "no-reply@crm.local"));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart("html") { Text = body };

                using var client = new SmtpClient();

                _logger.LogInformation("Próba połączenia z serwerem SMTP: {Host}:{Port}", _settings.Host, _settings.Port);

                await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.None, ct);

                if (!string.IsNullOrEmpty(_settings.Username))
                {
                    await client.AuthenticateAsync(_settings.Username, _settings.Password, ct);
                }

                await client.SendAsync(email, ct);
                await client.DisconnectAsync(true, ct);

                _logger.LogInformation("E-mail do {To} został wysłany pomyślnie.", to);
                return true;
            });
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "KRYTYCZNY BŁĄD: Nie udało się wysłać maila do {To} po wszystkich próbach.", to);
                return false;
            }
        }

    }
}
