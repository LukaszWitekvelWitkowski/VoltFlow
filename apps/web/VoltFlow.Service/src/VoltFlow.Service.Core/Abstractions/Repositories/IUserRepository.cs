using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.User.DTO;

namespace VoltFlow.Service.Core.Abstractions.Repositories
{
    public interface IUserRepository
    {
        Task<UserDto?> GetUserDtoByEmailAsync(string email);

        Task<User?> GetByEmailAsync(string email);
        Task UpdateAsync(User user, string? usedTokenHash = null);
    }
}
