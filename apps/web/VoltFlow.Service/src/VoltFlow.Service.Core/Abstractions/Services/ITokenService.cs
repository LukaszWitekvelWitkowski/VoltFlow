using VoltFlow.Service.Core.Entities;

namespace VoltFlow.Service.Core.Abstractions.Services
{
    public interface ITokenService
    {
        string GenerateToken();
        string HashToken(string token);
    }
}
