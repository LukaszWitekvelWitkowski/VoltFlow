using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Enums;
using VoltFlow.Service.Core.Models.User.DTO;

namespace VoltFlow.Service.Core.Mapper
{
    public static class UserMapper
    {
        public static User ToEntity(this UserDto dto)
        {
            return new User
            {
                Id = dto.Id,
                Name = dto.Name,
                Email = dto.Email,
                UserName = dto.Email,
                NormalizedEmail = dto.Email?.ToUpper(),
                NormalizedUserName = dto.Email?.ToUpper(),
                RoleId = dto.RoleId,
                PasswordHash = dto.PasswordHash,
                // Ustawienie wartości domyślnych dla pól, których nie ma w DTO
                Status = UserStatus.Active,
                TenantId = 1 // Przykładowo, docelowo powinno być dynamiczne
            };
        }
    }
}
