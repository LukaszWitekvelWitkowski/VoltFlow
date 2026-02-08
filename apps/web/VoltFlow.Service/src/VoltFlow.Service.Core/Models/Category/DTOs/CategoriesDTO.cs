namespace VoltFlow.Service.Core.Models.Category.DTOs
{
    public class CategoriesDTO
    {
        public IEnumerable<CategoryDTO> Categories { get; set; }

        public CategoriesDTO(IEnumerable<CategoryDTO>? categories)
        {
            if (categories == null)
            {
                Categories = new List<CategoryDTO>();
                return;
            }
            Categories = categories;
        }
    }
}
