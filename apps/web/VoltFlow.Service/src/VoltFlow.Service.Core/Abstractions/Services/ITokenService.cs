using VoltFlow.Service.Core.Entities;

namespace VoltFlow.Service.Core.Abstractions.Services
{
    public interface ITokenService
    {
        string GeneratePasswordResetToken(User user);
        string HashToken(string token);
    }
}
