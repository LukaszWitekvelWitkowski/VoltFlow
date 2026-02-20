using VoltFlow.Service.Core.Abstractions.Generic;

namespace VoltFlow.Service.Core.Models.Element.DTOs
{
    public class ElementsDTO : ICacheData<ElementDTO>
    {
        public IEnumerable<ElementDTO> Items { get; set; } = new List<ElementDTO>();

        public void insert(IEnumerable<ElementDTO> enumerable)
        {
            Items = enumerable;

        }
    }
}
