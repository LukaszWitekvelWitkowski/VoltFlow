using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
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
    public class VerifyEmailHandler : IRequestHandler<VerifyEmailCommand, ServiceResponse<Result>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public VerifyEmailHandler(
            IUserRepository userRepository,
            ITokenRepository tokenRepository,
            ITokenService tokenService,
            UserManager<User> userManager,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<Result>> Handle(VerifyEmailCommand command, CancellationToken ct)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {

                // 1. Znajdź użytkownika
                var user = await _userRepository.GetByEmailAsync(command.request.Email);
                if (user == null) return ServiceResponse<Result>.Failure("User not found.");
                if (user.IsEmailVerified) return ServiceResponse<Result>.Failure("Email already verified.");

                // 2. Pobierz i zweryfikuj token
                var tokenRecord = await GetValidToken(user.Id, command.request.Token, ct);
                if (tokenRecord == null) return ServiceResponse<Result>.Failure("Invalid or expired token.");

                // 3. Zaktualizuj status użytkownika
                var updateResult = await MarkUserAsVerified(user);
                if (!updateResult._IsSuccess) return updateResult;

                // 4. Unieważnij użyty token
                await InvalidateToken(tokenRecord, ct);

                return ServiceResponse<Result>.Success(Result.isSucces());
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ServiceResponse<Result>.Failure($"An error occurred: {ex.Message}", 500);
            }
            finally
            {
                await _unitOfWork.CommitTransactionAsync();
            }
        }

        #region Private Methods

        private async Task<VerificationToken?> GetValidToken(int userId, string rawToken, CancellationToken ct)
        {
            string hashedToken = _tokenService.HashToken(rawToken);

            // Szukamy tokena dla tego użytkownika, o typie EmailConfirmation, który nie wygasł i nie został użyty
            return await _tokenRepository.GetActiveTokenAsync(
                userId,
                hashedToken,
                TokenType.EmailConfirmation,
                ct);
        }

        private async Task<ServiceResponse<Result>> MarkUserAsVerified(User user)
        {
            user.IsEmailVerified = true;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return ServiceResponse<Result>.Failure(result.Errors.FirstOrDefault()?.Description ?? "Update failed.");
            }

            return ServiceResponse<Result>.Success(Result.isSucces());
        }

        private async Task InvalidateToken(VerificationToken token, CancellationToken ct)
        {
            token.IsUsed = true;
            token.UsedAt = DateTime.UtcNow;
            await _tokenRepository.UpdateAsync(token, ct);
        }

        #endregion
    }
}
