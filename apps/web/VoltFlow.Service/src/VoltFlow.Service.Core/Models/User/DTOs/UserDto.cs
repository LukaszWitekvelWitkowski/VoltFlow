namespace VoltFlow.Service.Core.Models.User.DTO
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string PasswordHash { get; set; } = null!;


        public string? PasswordResetTokenHash { get; set; }
        public DateTime? ResetTokenExpires { get; set; }



    }
}
