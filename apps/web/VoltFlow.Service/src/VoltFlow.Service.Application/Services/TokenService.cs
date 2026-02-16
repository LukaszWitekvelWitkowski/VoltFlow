using System.Security.Cryptography;
using System.Text;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Entities;

namespace VoltFlow.Service.Application.Services
{
    public class TokenService : ITokenService
    {
        public string GeneratePasswordResetToken(User user)
        {
            // Generate 32 random bytes
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            // Convert to a secure Base64 string (remove special characters for URLs)
            return Convert.ToBase64String(randomNumber)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
        }

        public string HashToken(string token)
        {
            // Always store the HASH token in the database, not the raw token.
            // If the database is leaked, the hacker won't reset users' passwords.
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}
