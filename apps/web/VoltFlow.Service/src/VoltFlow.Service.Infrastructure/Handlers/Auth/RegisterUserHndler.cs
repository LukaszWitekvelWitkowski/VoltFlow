using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VoltFlow.Service.Application.Commands.Auth;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Handlers.Auth
{
    public class RegisterUserHndler : IRequestHandler<RegisterUserCommand, ServiceResponse<Result>>
    {
        private readonly IAuthService _authService;
        private readonly UserManager<User> _userManager;
        private readonly VoltFlowDbContext _context; // Zakładamy, że mamy dostęp do DbContext, np. przez DI

        public RegisterUserHndler(IAuthService authService, UserManager<User> userManager, VoltFlowDbContext context)
        {
            _authService = authService;
            _userManager = userManager;
            _context = context;
        }

        public async Task<ServiceResponse<Result>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {

            // Rozpoczynamy transakcję na poziomie bazy danych
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // 1. Sprawdzenie czy użytkownik istnieje
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                    return ServiceResponse<Result>.Failure("Użytkownik już istnieje.");

                // 2. Tworzenie encji
                var user = new User { UserName = request.Email,  Name = request.Login, Email = request.Email, RoleId = 1 };

                // 3. Zapis do bazy przez Identity
                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken); // Wycofujemy w razie błędu walidacji Identity
                    return ServiceResponse<Result>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
                }

              //  await _userManager.AddToRoleAsync(user, "User");

                // 4. Logika dodatkowa (np. wysyłka maila lub zapisanie zadania wysyłki do bazy)
              /*  var mailResult = await _authService.SendMainVerification(user);
                if (!mailResult._IsSuccess)
                {
                    // Jeśli mail jest krytyczny dla procesu - robimy Rollback.
                    // Jeśli nie, możemy mimo wszystko zrobić Commit. Tu zakładamy, że jest krytyczny.
                    await transaction.RollbackAsync(cancellationToken);
                    return ServiceResponse<Result>.Failure("Błąd podczas inicjacji weryfikacji mailowej.");
                }*/

                // 5. Dopiero tutaj zatwierdzamy wszystko w bazie
                await transaction.CommitAsync(cancellationToken);

                return ServiceResponse<Result>.Success(new Result(true));
            }
            catch (Exception ex)
            {
                // W razie nieprzewidzianego błędu (np. bazy danych) wycofujemy zmiany
                await transaction.RollbackAsync(cancellationToken);
                // Log ex...
                return ServiceResponse<Result>.Failure("Wystąpił nieoczekiwany błąd podczas rejestracji.");
            }

        }
    }
}
