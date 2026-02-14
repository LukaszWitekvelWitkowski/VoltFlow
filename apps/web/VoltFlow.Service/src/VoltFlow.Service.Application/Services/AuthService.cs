using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Services
{
    public class AuthService : IAuthService
    {
        public AuthService()
        {

        }

        Task<ServiceResponse<ServiceResponse<bool>>> IAuthService.SendMainVerification(User user)
        {
            throw new NotImplementedException();
        }
    }
}
