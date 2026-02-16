using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.User.DTO;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(VoltFlowDbContext context, IConfiguration configuration) : base(context, configuration)
        {
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            return _context.Set<User>().FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserDto?> GetUserDtoByEmailAsync(string email)
        {
            return await _context.Set<User>()
                    .Where(u => u.Email == email)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Email = u.Email,
                        RoleId = u.RoleId,

                        PasswordResetTokenHash = _context.UserPasswordResets
                            .Where(r => r.UserId == u.Id && r.UsedAt == null)
                            .OrderByDescending(r => r.CreatedAt)
                            .Select(r => r.TokenHash)
                            .FirstOrDefault(),

                        ResetTokenExpires = _context.UserPasswordResets
                            .Where(r => r.UserId == u.Id && r.UsedAt == null)
                            .OrderByDescending(r => r.CreatedAt)
                            .Select(r => r.ExpiresAt)
                            .FirstOrDefault()
                    })
                    .FirstOrDefaultAsync();
        }


        public async Task UpdateAsync(User user, string? usedTokenHash = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.users.Update(user);

                if (!string.IsNullOrEmpty(usedTokenHash))
                {
                    var resetToken = await _context.UserPasswordResets
                        .FirstOrDefaultAsync(r => r.UserId == user.Id && r.TokenHash == usedTokenHash && r.UsedAt == null);

                    if (resetToken != null)
                    {
                        resetToken.UsedAt = DateTime.UtcNow;
                        _context.UserPasswordResets.Update(resetToken);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
