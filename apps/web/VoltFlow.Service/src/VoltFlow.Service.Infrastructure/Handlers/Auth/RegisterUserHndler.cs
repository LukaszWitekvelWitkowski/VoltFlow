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
        private readonly VoltFlowDbContext _context; 

        public RegisterUserHndler(IAuthService authService, UserManager<User> userManager, VoltFlowDbContext context)
        {
            _authService = authService;
            _userManager = userManager;
            _context = context;
        }

        public async Task<ServiceResponse<Result>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {

            // Start a transaction at the database level

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // 1. Check if the user exists
                var existingUser = await _userManager.FindByEmailAsync(request.RegisterUserRequest.Email);
                if (existingUser != null)
                    return ServiceResponse<Result>.Failure("Użytkownik już istnieje.");

                // 2. Creating an entity 
                var user = new User { UserName = request.RegisterUserRequest.Email, Name = request.RegisterUserRequest.Login, Email = request.RegisterUserRequest.Email, RoleId = request.Role };

                // 3. Saving to the database via Identity 
                var result = await _userManager.CreateAsync(user, request.RegisterUserRequest.Password);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken); // Rollback on Identity validation error 
                    return ServiceResponse<Result>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                // await _userManager.AddToRoleAsync(user, "User"); 

                // 4. Additional logic (e.g., sending an email or saving the sending task to the database)
                /* var mailResult = await _authService.SendMainVerification(user);

                if (!mailResult._IsSuccess)
                {
                // If the email is critical to the process, we rollback.
                // If not, we can still commit. Here we assume it is critical.

                await transaction.RollbackAsync(cancellationToken);

                return ServiceResponse<Result>.Failure("Error initiating email verification.");

                }*/

                // 5. Only then do we commit everything to the database

                await transaction.CommitAsync(cancellationToken);

                return ServiceResponse<Result>.Success(new Result(true));
            }
            catch (Exception ex)
            {
                // In the event of an unforeseen error (e.g., database error), we roll back the changes.
                await transaction.RollbackAsync(cancellationToken);
                // Log ex...
                return ServiceResponse<Result>.Failure("Wystąpił nieoczekiwany błąd podczas rejestracji.");
            }

        }
    }
}
