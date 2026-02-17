using MediatR;
using VoltFlow.Service.Application.Commands.Email;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Enums;
using VoltFlow.Service.Core.Helper;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Infrastructure.Handlers.Email
{
    public class SendOverduePaymentHandler : IRequestHandler<SendOverduePaymentCommand, ServiceResponse<Result>>
    {
        private readonly IEmailRepository _emailRepository; // Zmienione z DbContext
        private readonly IEmailService _emailService;

        public SendOverduePaymentHandler(IEmailRepository emailRepository, IEmailService emailService)
        {
            _emailRepository = emailRepository;
            _emailService = emailService;
        }

        public async Task<ServiceResponse<Result>> Handle(SendOverduePaymentCommand request, CancellationToken ct)
        {
            var template = await _emailRepository.GetTemplateByTypeAsync(EmailTypeEnum.OverduePayment, ct);

            if (template == null)
            {
                return ServiceResponse<Result>.Failure("Brak szablonu email dla zaległej płatności. Skontaktuj się z administratorem systemu.");
            }

            var body = TemplateHelper.FormatTemplate(template.BodyHtml, request);


            var success = await _emailService.SendEmailAsync(request.CustomerEmail, template.Subject, body, ct);

        
            await _emailRepository.AddLogAsync(new EmailLog
            {
                To = request.CustomerEmail,
                Subject = template.Subject,
                IsSuccess = success,
                EmailType = EmailTypeEnum.OverduePayment,
                RelatedClientId = request.ClientId,
                ErrorMessage = success ? null : "Błąd wysyłki - sprawdź logi systemowe."
            }, ct);

            await _emailRepository.SaveChangesAsync(ct);

            return ServiceResponse<Result>.Success(new Result(true));
        }
    }
}
