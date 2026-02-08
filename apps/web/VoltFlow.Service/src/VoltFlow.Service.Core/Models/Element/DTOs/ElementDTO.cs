namespace VoltFlow.Service.Core.Models.Element.DTOs
{
    public class ElementDTO
    {
        public int IdElement { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsObsolete { get; set; }
        public int ElementGroupId { get; set; }
    }
}
