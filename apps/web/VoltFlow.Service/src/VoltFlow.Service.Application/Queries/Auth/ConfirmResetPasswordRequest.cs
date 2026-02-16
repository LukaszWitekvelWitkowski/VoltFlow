namespace VoltFlow.Service.Application.Queries.Auth
{
    public class ConfirmResetPasswordRequest
    {
        public required string Email { get; set; }
        public required string Token { get; set; }
        public required string NewPassword { get; set; }
    }
}
