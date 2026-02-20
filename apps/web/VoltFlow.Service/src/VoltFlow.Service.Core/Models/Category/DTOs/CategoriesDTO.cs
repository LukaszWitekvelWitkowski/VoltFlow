using VoltFlow.Service.Core.Abstractions.Generic;

namespace VoltFlow.Service.Core.Models.Category.DTOs
{
    public class CategoriesDTO : ICacheData<CategoryDTO>
    {
        public IEnumerable<CategoryDTO> Items { get; set; } = new List<CategoryDTO>();

        public void insert(IEnumerable<CategoryDTO> items)
        {
            Items = items;
        }
    }
}
