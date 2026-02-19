using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Polly;
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
    public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, ServiceResponse<Result>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ITokenRepository _tokenRepository;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public ConfirmEmailHandler(IUserRepository userRepository, ITokenService tokenService, IEmailSender emailSender, ITokenRepository tokenRepository, UserManager<User> manager, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _emailSender = emailSender;
            _tokenRepository = tokenRepository;
            _userManager = manager;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<Result>> Handle(ConfirmEmailCommand command, CancellationToken ct)
        {
            // 1. Walidacja użytkownika
            var user = await _userRepository.GetByEmailAsync(command.request.Email);
            var validationResult = ValidateUserForVerification(user);
            if (!validationResult.IsSuccess) return validationResult;

            // 2. Generowanie i zapisywanie tokena
            var (rawToken, hashedToken) = GenerateTokens();
            await SaveVerificationToken(user.Id, hashedToken, ct);

            // 3. Wysyłka wiadomości E-mail
            var emailSent = await SendVerificationEmail(user, rawToken, ct);
            if (!emailSent)
            {
                return ServiceResponse<Result>.Failure("Failed to send verification message.");
            }

            // 4. Aktualizacja statusu użytkownika
            return await UpdateUserEmailStatus(user);
        }

        #region Private Methods

        private ServiceResponse<Result> ValidateUserForVerification(User? user)
        {
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                return ServiceResponse<Result>.Failure("Unable to find user.");
            }

            if (user.IsEmailVerified)
            {
                return ServiceResponse<Result>.Failure("Email is already verified.");
            }

            return ServiceResponse<Result>.Success(null!); // Tylko do wewnętrznej walidacji
        }

        private (string raw, string hashed) GenerateTokens()
        {
            string raw = _tokenService.GenerateToken();
            string hashed = _tokenService.HashToken(raw);
            return (raw, hashed);
        }

        private async Task SaveVerificationToken(int userId, string hashedToken, CancellationToken ct)
        {
            var verification = VerificationToken.Create(hashedToken, userId, TokenType.EmailConfirmation);
            await _tokenRepository.AddAsync(verification, ct);
        }

        private async Task<bool> SendVerificationEmail(User user, string rawToken, CancellationToken ct)
        {
            string baseUrl = _configuration["AppSettings:ClientBaseUrl"] ?? "https://localhost:7199";
            string verificationLink = $"{baseUrl}/verify-email?token={rawToken}&email={user.Email}";

            return await _emailSender.SendTemplatedEmailAsync(
                user.Email!,
                EmailTypeEnum.Verification,
                new
                {
                    UserName = user.Name,
                    VerificationLink = verificationLink,
                    Token = rawToken
                },
                user.Id,
                ct);
        }

        private async Task<ServiceResponse<Result>> UpdateUserEmailStatus(User user)
        {
            user.IsSendEmailVeryfied = true; // Zachowuję Twoją nazwę pola z literówką

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault()?.Description ?? "Identity Update Error";
                return ServiceResponse<Result>.Failure(error);
            }

            return ServiceResponse<Result>.Success(Result.isSucces());
        }

        #endregion
    }
}
