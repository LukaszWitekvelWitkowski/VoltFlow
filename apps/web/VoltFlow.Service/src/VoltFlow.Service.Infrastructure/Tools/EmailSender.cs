using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Abstractions.Tools;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Enums;
using VoltFlow.Service.Core.Helper;

namespace VoltFlow.Service.Infrastructure.Tools
{
    public class EmailSender : IEmailSender
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IEmailService _emailService;

        public EmailSender(IEmailRepository emailRepository, IEmailService emailService)
        {
            _emailRepository = emailRepository;
            _emailService = emailService;
        }

        public async Task<bool> SendTemplatedEmailAsync<TModel>(
            string to,
            EmailTypeEnum type,
            TModel model,
            int? clientId = null,
            CancellationToken ct = default)
        {
            // 1. Pobranie szablonu
            var template = await _emailRepository.GetTemplateByTypeAsync(type, ct);
            if (template == null) return false;

            // 2. Formatowanie (korzystamy z Twojego Helpera)
            var body = TemplateHelper.FormatTemplate(template.BodyHtml, model);

            // 3. Wysyłka
            var success = await _emailService.SendEmailAsync(to, template.Subject, body, ct);

            // 4. Logowanie (enkapsulujemy logikę logowania tutaj)
            await _emailRepository.AddLogAsync(new EmailLog
            {
                To = to,
                Subject = template.Subject,
                IsSuccess = success,
                EmailType = type,
                RelatedClientId = clientId,
                ErrorMessage = success ? null : "Błąd wysyłki - sprawdź logi serwisu e-mail."
            }, ct);

            return success;
        }
    }
}
