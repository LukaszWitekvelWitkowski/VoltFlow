using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Core.Abstractions.Services
{
    public interface IAuthService
    {
        Task<ServiceResponse<ServiceResponse<bool>>> SendMainVerification(User user);
    }
}
