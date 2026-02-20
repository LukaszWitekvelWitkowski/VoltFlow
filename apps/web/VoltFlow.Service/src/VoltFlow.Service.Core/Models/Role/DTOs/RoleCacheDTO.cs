using VoltFlow.Service.Core.Abstractions.Generic;

namespace VoltFlow.Service.Core.Models.Role.DTOs
{
    public class RoleCacheDTO : ICacheData<RoleDto>
    {
        public IEnumerable<RoleDto> Items { get; set; } = new List<RoleDto>();

        public void insert(IEnumerable<RoleDto> enumerable)
        {
            Items = enumerable;
        }
    }
}
