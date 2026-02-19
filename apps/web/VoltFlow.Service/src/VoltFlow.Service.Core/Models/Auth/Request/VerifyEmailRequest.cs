namespace VoltFlow.Service.Core.Models.Auth.Request
{
    public class VerifyEmailRequest
    {
        public VerifyEmailRequest(string email, string token)
        {
            Email = email;
            Token = token;
        }

        public  string Email { get;  }
        public  string Token { get;  }
    }
}
