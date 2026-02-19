namespace VoltFlow.Service.Core.Models.Auth.Request
{
    public class VerifyEmailRequest
    {
        public required string Email { get; set; }
        public required string Token { get; set; }
    }
}
