using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Entities
{
    public class VerificationToken
    {
        public int Id { get; private set; }
        public string TokenHash { get; private set; } = null!;
        public int UserId { get; private set; }
        public TokenType Type { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsUsed { get; set; }

        public virtual User User { get; set; } = null!;
        public DateTime UsedAt { get; set; }

        public static VerificationToken Create(string tokenHash, int userId, TokenType type, int expiryHours = 24)
        {
            return new VerificationToken
            {
                TokenHash = tokenHash,
                UserId = userId,
                Type = type,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(expiryHours),
                IsUsed = false
            };
        }
    }
}