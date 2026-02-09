namespace VoltFlow.Service.Core.Models.ElementGroup.DTOs
{
    public class ElementGroupDTO
    {
        public int IdElementGroup { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsObsolete { get; set; }
        public int CategoryId { get; set; }
    }
}
