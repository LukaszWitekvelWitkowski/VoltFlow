using MediatR;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Auth.Request;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Commands.Auth
{
    public record CompleteClientOnboardingCommand : IRequest<ServiceResponse<Result>>
    {
        public CompleteClientOnboardingCommand(CompleteOnboardingRequest request)
        {
            this.request = request;
        }

        public CompleteOnboardingRequest request { get; }
    }
}
