namespace VoltFlow.Service.Core.Models.Category
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsObsolete { get; set; }
    }
}
