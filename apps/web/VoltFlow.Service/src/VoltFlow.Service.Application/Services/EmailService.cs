using VoltFlow.Service.Core.Abstractions.Services;

namespace VoltFlow.Service.Application.Services
{
    public class EmailService : IEmailService
    {
        public Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            throw new NotImplementedException();
        }
    }
}
