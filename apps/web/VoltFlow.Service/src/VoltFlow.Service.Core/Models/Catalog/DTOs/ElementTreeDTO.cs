namespace VoltFlow.Service.Core.Models.Catalog.DTOs
{

    public class ElementTreeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Zagnieżdżenie "w górę" zgodnie z Twoim opisem powielania
        public ElementGroupSimpleDTO? Group { get; set; }
    }

    public class ElementGroupSimpleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public CategorySimpleDTO? Category { get; set; }
    }

    public class CategorySimpleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
