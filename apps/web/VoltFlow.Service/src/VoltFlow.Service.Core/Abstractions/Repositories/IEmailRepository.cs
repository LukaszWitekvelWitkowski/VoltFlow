using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Abstractions.Repositories
{
    public interface IEmailRepository
    {
        Task<EmailTemplate?> GetTemplateByTypeAsync(EmailTypeEnum type, CancellationToken ct = default);
        Task AddLogAsync(EmailLog log, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
