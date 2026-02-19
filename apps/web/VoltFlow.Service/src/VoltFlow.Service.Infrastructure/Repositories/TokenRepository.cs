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

        public async Task<VerificationToken?> GetActiveTokenAsync(string hashedToken, int userId, CancellationToken ct)
        {
            return await _context.Set<VerificationToken>()
                .Where(t => t.TokenHash == hashedToken)
                .Where(t => t.UserId == userId)
                .Where(t => t.Type == TokenType.EmailConfirmation) 
                .Where(t => t.ExpiresAt > DateTime.UtcNow)       
                .Where(t => !t.IsUsed)                            
                .FirstOrDefaultAsync(ct);                       
        }

        public void Remove(VerificationToken token)
        {
            _context.VerificationTokens.Remove(token);
        }
    }
}
