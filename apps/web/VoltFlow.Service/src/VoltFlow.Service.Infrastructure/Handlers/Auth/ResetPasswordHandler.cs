using MediatR;
// DODAJ TE USINGI (dostosuj ścieżki do swojego projektu):
using VoltFlow.Service.Application.Commands.Auth;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Abstractions.Tools;
using VoltFlow.Service.Core.Enums;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Infrastructure.Handlers.Auth
{
    // Zamieniłem Results na bool dla świętego spokoju
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, ServiceResponse<Result>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IEmailSender _emailSender;

        public ResetPasswordHandler(IUserRepository userRepository, ITokenService tokenService, IEmailSender emailSender)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _emailSender = emailSender;
        }

        public async Task<ServiceResponse<Result>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                return ServiceResponse<Result>.Failure("User not found.");
            }

            var token = _tokenService.GenerateToken();


            //    var resetLink = $"https://voltflow.pl/reset-password?token={Uri.EscapeDataString(token)}";

           var sucess = await _emailSender.SendTemplatedEmailAsync(
                user.Email,
                EmailTypeEnum.PasswordReset,
                new Dictionary<string, string>
                    {
                        { "ResetLink", $"https://voltflow.pl/reset-password?token={Uri.EscapeDataString(token)}" }
                    },
                user.Id,
                cancellationToken
                );


            if (!sucess)
            {
                return ServiceResponse<Result>.Failure("Failed to send email.");
            }

            return ServiceResponse<Result>.Success(Result.isSucces());
        }
    }
}