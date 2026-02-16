using System;
using System.Collections.Generic;
using System.Text;

namespace VoltFlow.Service.Core.Entities
{
    public class UserPasswordReset
    {
        public int IdReset { get; set; }
        public int UserId { get; set; }
        public string TokenHash { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UsedAt { get; set; }
        public string? IpAddress { get; set; }

        // Relacja do użytkownika
        public virtual User User { get; set; } = null!;
    }
}
