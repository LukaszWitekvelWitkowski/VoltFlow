namespace VoltFlow.Service.Core.Models.Category.Request
{
    public class UpdateCategoryRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsObsolete { get; set; }
    }
}
