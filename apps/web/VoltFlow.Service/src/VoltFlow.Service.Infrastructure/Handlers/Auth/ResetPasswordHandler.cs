using MediatR;
using Microsoft.Extensions.Configuration;

// DODAJ TE USINGI (dostosuj ścieżki do swojego projektu):
using VoltFlow.Service.Application.Commands.Auth;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Abstractions.Tools;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Enums;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Infrastructure.Handlers.Auth
{
    // Zamieniłem Results na bool dla świętego spokoju
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, ServiceResponse<Result>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly ITokenService _tokenService;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _clientBaseUrl;

        public ResetPasswordHandler(
                   IUserRepository userRepository,
                   ITokenRepository tokenRepository,
                   ITokenService tokenService,
                   IEmailSender emailSender,
                   IUnitOfWork unitOfWork,
                   IConfiguration configuration)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _tokenService = tokenService;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
            _clientBaseUrl = configuration["AppSettings:ClientBaseUrl"] ?? "https://localhost:7199";
        }

        public async Task<ServiceResponse<Result>> Handle(ResetPasswordCommand request, CancellationToken ct)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            // Bezpieczeństwo: Możesz rozważyć zwrócenie Success nawet jeśli usera nie ma, 
            // aby nie zdradzać istnienia konta w systemie.
            if (user == null) return ServiceResponse<Result>.Failure("User not found.");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1. Generowanie i haszowanie tokena
                var rawToken = _tokenService.GenerateToken();

                // 2. Zapis tokena do bazy (niezbędne do późniejszej weryfikacji!)
                await SaveResetToken(user.Id, rawToken, ct);

                // 3. Wysyłka e-maila
                var emailSent = await SendResetEmail(user, rawToken, ct);

                if (!emailSent)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ServiceResponse<Result>.Failure("Failed to send email.");
                }

                await _unitOfWork.CommitTransactionAsync();
                return ServiceResponse<Result>.Success(Result.isSucces());
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ServiceResponse<Result>.Failure($"An error occurred: {ex.Message}", 500);
            }
        }

        #region Private Methods

        private async Task SaveResetToken(int userId, string rawToken, CancellationToken ct)
        {
            var hashedToken = _tokenService.HashToken(rawToken);

            var tokenEntity = VerificationToken.Create(
                hashedToken,
                userId,
                TokenType.PasswordReset // Upewnij się, że masz taki typ w Enumie
            );

            await _tokenRepository.AddAsync(tokenEntity, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        private async Task<bool> SendResetEmail(User user, string rawToken, CancellationToken ct)
        {
            var resetLink = $"{_clientBaseUrl}/reset-password?token={Uri.EscapeDataString(rawToken)}";

            var placeholders = new Dictionary<string, string>
            {
                { "ResetLink", resetLink },
                { "UserName", user.UserName ?? "User" }
            };

            return await _emailSender.SendTemplatedEmailAsync(
                user.Email!,
                EmailType.PasswordReset,
                placeholders,
                user.Id,
                ct
            );
        }

        #endregion
    }
}