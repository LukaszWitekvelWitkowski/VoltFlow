using Microsoft.AspNetCore.Identity;
using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Entities
{
    public class User : IdentityUser<int>
    {
        public string Name { get; set; } = string.Empty;
        public int TenantId { get; set; }
        public UserStatus Status { get; set; }

        // Relacje
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
        public bool IsEmailVerified { get; set; }
        public bool IsSendEmailVeryfied { get; set; }
        public ICollection<UserPasswordReset> Users { get; set; } = new List<UserPasswordReset>();

    }
}
