using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Entities
{
    public class User
    {
        public int IdUser { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int TenantId { get; set; }
        public UserStatus Status { get; set; }

        // Relacje
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
