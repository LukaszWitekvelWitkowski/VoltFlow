using MediatR;
using VoltFlow.Service.Application.Commands.Auth;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Abstractions.Tools;
using VoltFlow.Service.Core.Enums;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Infrastructure.Handlers.Auth
{
    public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, ServiceResponse<Result>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ITokenRepository _tokenRepository;
        private readonly IEmailSender _emailSender;

        public ConfirmEmailHandler(IUserRepository userRepository, ITokenService tokenService, IEmailSender emailSender, ITokenRepository tokenRepository)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _emailSender = emailSender;
            _tokenRepository = tokenRepository;
        }

        public async Task<ServiceResponse<Result>> Handle(ConfirmEmailCommand command, CancellationToken ct)
        {
            var user = await _userRepository.GetByEmailAsync(command.request.Email);

            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                return ServiceResponse<Result>.Failure("Nie można znaleźć użytkownika o podanym adresie email.");
            }

            if (user.IsEmailVerified)
            {
                return ServiceResponse<Result>.Failure("Adres email jest już zweryfikowany.");
            }

            string rawToken = _tokenService.GenerateToken(); 
            string hashedToken = _tokenService.HashToken(rawToken);

            var tokenEntry = await _tokenRepository.GetActiveTokenAsync(hashedToken, user.Id, ct);

            if (tokenEntry == null)
            {
                return ServiceResponse<Result>.Failure("Link aktywacyjny jest nieprawidłowy lub wygasł.");
            }

            var success = await _emailSender.SendTemplatedEmailAsync<string>(
            user.Email,
            EmailTypeEnum.Verification,
            tokenEntry.TokenHash, 
            user.Id,
            ct);

            if (!success)
            {
                return ServiceResponse<Result>.Failure("Nie udało się wysłać wiadomości.");
            }

            user.IsSendEmailVeryfied = true;

            _tokenRepository.Remove(tokenEntry);

            await _userRepository.UpdateAsync(user);

            return ServiceResponse<Result>.Success(new Result(true));
        }
    }
}
