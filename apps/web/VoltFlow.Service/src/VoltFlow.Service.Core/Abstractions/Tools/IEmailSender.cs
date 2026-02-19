using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Abstractions.Tools
{
    public interface IEmailSender
    {
        Task<bool> SendTemplatedEmailAsync<TModel>(
        string to,
        EmailTypeEnum type,
        TModel model,
        int? clientId = null,
        CancellationToken ct = default);
    }
}
