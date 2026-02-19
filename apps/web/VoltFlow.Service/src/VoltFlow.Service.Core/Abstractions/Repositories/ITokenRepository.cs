using VoltFlow.Service.Core.Entities;

namespace VoltFlow.Service.Core.Abstractions.Repositories
{
    public interface ITokenRepository
    {
        Task<VerificationToken?> GetActiveTokenAsync(string hashedToken, int userId, CancellationToken ct);

        void Remove(VerificationToken token);
    }
}
