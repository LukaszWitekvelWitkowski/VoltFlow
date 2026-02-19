using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Enums;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class TokenRepository : BaseRepository, ITokenRepository
    {
        public TokenRepository(VoltFlowDbContext context, IConfiguration configuration) : base(context, configuration)
        {
        }

        public async Task AddAsync(VerificationToken token, CancellationToken ct)
        {
            await _context.VerificationTokens.AddAsync(token, ct);
            await _context.SaveChangesAsync(ct);
        }


        public async Task<VerificationToken?> GetActiveTokenAsync(int userId, string hashedToken, TokenType type, CancellationToken ct)
        {
            return await _context.VerificationTokens
                         .FirstOrDefaultAsync(t =>
                             t.UserId == userId &&
                             t.TokenHash == hashedToken &&
                             t.Type == type &&
                             !t.IsUsed &&
                             t.ExpiresAt > DateTime.UtcNow,
                         ct);
        }

        public void Remove(VerificationToken token)
        {
            _context.VerificationTokens.Remove(token);
        }

        public async Task UpdateAsync(VerificationToken token, CancellationToken ct)
        {
            _context.VerificationTokens.Update(token);
            await _context.SaveChangesAsync(ct);
        }
    }
}
