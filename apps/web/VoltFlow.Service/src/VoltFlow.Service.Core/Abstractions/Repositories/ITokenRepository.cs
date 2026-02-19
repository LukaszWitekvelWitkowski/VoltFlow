using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Abstractions.Repositories
{
    public interface ITokenRepository
    {
        Task AddAsync(VerificationToken token, CancellationToken ct);
        Task<VerificationToken?> GetActiveTokenAsync(int userId, string hashedToken, TokenType type, CancellationToken ct);

        void Remove(VerificationToken token);
        Task UpdateAsync(VerificationToken token, CancellationToken ct);
    }
}
