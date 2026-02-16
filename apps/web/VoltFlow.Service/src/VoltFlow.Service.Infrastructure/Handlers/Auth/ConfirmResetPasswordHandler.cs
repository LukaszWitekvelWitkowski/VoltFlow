using MediatR;
using Microsoft.AspNetCore.Identity;
using VoltFlow.Service.Application.Commands.Auth;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Mapper;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Infrastructure.Handlers.Auth
{
    public class ConfirmResetPasswordHandler : IRequestHandler<ConfirmResetPasswordCommand, ServiceResponse<Result>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher<User> _passwordHasher; // Z wbudowanego Identity lub własny

        public async Task<ServiceResponse<Result>> Handle(ConfirmResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserDtoByEmailAsync(request.confirmResetPasswordRequest.Email);
            if (user == null) return ServiceResponse<Result>.Failure("Błędne dane.");

            // 1. Hashujemy token przysłany z frontendu, żeby porównać go z tym w bazie
            var incomingHash = _tokenService.HashToken(request.confirmResetPasswordRequest.Token);

            // 2. Weryfikacja
            if (user.PasswordResetTokenHash != incomingHash || user.ResetTokenExpires < DateTime.UtcNow)
            {
                return ServiceResponse<Result>.Failure("Token wygasł lub jest nieprawidłowy.");
            }

            var userEntity = UserMapper.ToEntity(user);

            // 3. Zmiana hasła
            user.PasswordHash = _passwordHasher.HashPassword(userEntity, request.confirmResetPasswordRequest.NewPassword);

            // 4. Czyścimy token, żeby nie można go było użyć drugi raz!
            user.PasswordResetTokenHash = null;
            user.ResetTokenExpires = null;

            await _userRepository.UpdateAsync(userEntity);

            return ServiceResponse<Result>.Success( new Result(true));
        }
    }
}
