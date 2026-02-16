using MediatR;
// DODAJ TE USINGI (dostosuj ścieżki do swojego projektu):
using VoltFlow.Service.Application.Commands.Auth;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Infrastructure.Handlers.Auth
{
    // Zamieniłem Results na bool dla świętego spokoju
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, ServiceResponse<Result>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public ResetPasswordHandler(IUserRepository userRepository, ITokenService tokenService, IEmailService emailService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        public async Task<ServiceResponse<Result>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                return ServiceResponse<Result>.Failure("User not found.");
            }

            var token = _tokenService.GeneratePasswordResetToken(user);


       //    var resetLink = $"https://voltflow.pl/reset-password?token={Uri.EscapeDataString(token)}";

       /*     var emailResult = await _emailService.SendEmailAsync(user.Email, "Password Reset", $"Click the link: {resetLink}");

            if (!emailResult)
            {
                return ServiceResponse<Result>.Failure("Failed to send email.");
            }
       */
            return ServiceResponse<Result>.Success(new Result(true));
        }
    }
}