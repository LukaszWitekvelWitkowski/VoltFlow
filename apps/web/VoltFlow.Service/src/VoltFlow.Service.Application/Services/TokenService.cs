using System.Security.Cryptography;
using System.Text;
using VoltFlow.Service.Core.Abstractions.Services;

namespace VoltFlow.Service.Application.Services
{
    public class TokenService : ITokenService
    {
        public string GenerateToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            // Używamy Base64Url (bez +, / i =), aby token był bezpieczny w linkach URL
            return Convert.ToBase64String(randomNumber)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        public string HashToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be empty", nameof(token));

            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));

            // Zwracamy czysty Hex string
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}
