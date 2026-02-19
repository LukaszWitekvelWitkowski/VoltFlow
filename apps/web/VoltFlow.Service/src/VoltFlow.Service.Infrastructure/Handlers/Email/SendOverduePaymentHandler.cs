using MediatR;
using VoltFlow.Service.Application.Commands.Email;
using VoltFlow.Service.Core.Abstractions.Tools;
using VoltFlow.Service.Core.Enums;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Infrastructure.Handlers.Email
{
    public class SendOverduePaymentHandler : IRequestHandler<SendEmailCommand, ServiceResponse<Result>>
    {
        private readonly IEmailSender _emailSender;

        public SendOverduePaymentHandler(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task<ServiceResponse<Result>> Handle(SendEmailCommand request, CancellationToken ct)
        {
            try
            {
                var success = await _emailSender.SendTemplatedEmailAsync(
                    request.CustomerEmail,
                    EmailType.OverduePayment,
                    request,
                    request.ClientId,
                    ct);

                if (!success)
                {
                    return ServiceResponse<Result>.Failure("Failed to send message.");
                }

                return ServiceResponse<Result>.Success(Result.isSucces());
            }
            catch (Exception ex)
            {
                return ServiceResponse<Result>.Failure(ex.Message, 500);
            }
        }
    }
}
