using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly string _baseUrl;

        public ConfirmEmailHandler(IUserRepository userRepository, ITokenService tokenService, IEmailSender emailSender, ITokenRepository tokenRepository, UserManager<User> manager, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _emailSender = emailSender;
            _tokenRepository = tokenRepository;
            _userManager = manager;
            _baseUrl = configuration["AppSettings:ClientBaseUrl"] ?? "https://localhost:7199";
        }

        public async Task<ServiceResponse<Result>> Handle(ConfirmEmailCommand command, CancellationToken ct)
        {

            var user = await _userRepository.GetByEmailAsync(command.request.Email);
            var validationResult = ValidateUserForVerification(user);
            if (!validationResult._IsSuccess) return validationResult;

            var (rawToken, hashedToken) = GenerateTokens();
            await SaveVerificationToken(user.Id, hashedToken, ct);

            var emailSent = await SendVerificationEmail(user, rawToken, ct);
            if (!emailSent)
            {
                return ServiceResponse<Result>.Failure("Failed to send verification message.");
            }
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

            return ServiceResponse<Result>.Success(null!); 
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

            string verificationLink = $"{_baseUrl}/verify-email?token={rawToken}&email={user.Email}";

            return await _emailSender.SendTemplatedEmailAsync(
                user.Email!,
                EmailType.Verification,
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
            user.IsSendEmailVeryfied = true; 

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
