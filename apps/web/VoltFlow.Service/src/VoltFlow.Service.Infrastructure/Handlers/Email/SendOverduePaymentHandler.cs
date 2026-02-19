using MediatR;
using VoltFlow.Service.Application.Commands.Email;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Abstractions.Tools;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Enums;
using VoltFlow.Service.Core.Helper;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Infrastructure.Handlers.Email
{
    public class SendOverduePaymentHandler : IRequestHandler<SendOverduePaymentCommand, ServiceResponse<Result>>
    {
        private readonly IEmailSender _emailSender;

        public SendOverduePaymentHandler(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task<ServiceResponse<Result>> Handle(SendOverduePaymentCommand request, CancellationToken ct)
        {
            var success = await _emailSender.SendTemplatedEmailAsync(
             request.CustomerEmail,
             EmailTypeEnum.OverduePayment,
             request,
             request.ClientId,
             ct);

            if (!success)
            {
                return ServiceResponse<Result>.Failure("Nie udało się wysłać wiadomości.");
            }

            return ServiceResponse<Result>.Success(new Result(true));
        }
    }
}
