using VoltFlow.Service.Core.Abstractions.Generic;

namespace VoltFlow.Service.Core.Models.ElementGroup.DTOs
{
    public class ElementGroupsDTO : ICacheData<ElementGroupDTO>
    {
        public IEnumerable<ElementGroupDTO> Items { get; set; } = new List<ElementGroupDTO>();

        public void insert(IEnumerable<ElementGroupDTO> enumerable)
        {
            Items = enumerable;
        }
    }
}
