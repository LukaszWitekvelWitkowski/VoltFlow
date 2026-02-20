namespace VoltFlow.Service.Core.Models.Auth.Request
{
    public class RegisterUserRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public required string Login { get; set; }
    }
}
