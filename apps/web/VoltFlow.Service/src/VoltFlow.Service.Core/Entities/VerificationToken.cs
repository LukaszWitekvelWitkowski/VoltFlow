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
        public bool IsUsed { get; private set; }

        public virtual User User { get; set; } = null!;
    }
}