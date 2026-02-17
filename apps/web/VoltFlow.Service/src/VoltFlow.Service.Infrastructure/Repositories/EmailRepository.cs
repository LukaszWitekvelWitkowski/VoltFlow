using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Enums;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class EmailRepository : BaseRepository, IEmailRepository
    {
        public EmailRepository(VoltFlowDbContext context, IConfiguration configuration) : base(context, configuration)
        {
        }

        public async Task AddLogAsync(EmailLog log, CancellationToken ct = default)
        {
            await _context.EmailLogs.AddAsync(log, ct);
        }

        public async Task<EmailTemplate?> GetTemplateByTypeAsync(EmailTypeEnum type, CancellationToken ct = default)
        {
            return await _context.EmailTemplates
            .FirstOrDefaultAsync(t => t.EmailType == type, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
