namespace VoltFlow.Service.Core.Entities
{
    public class ElementGroup
    {
        public int IdElementGroup { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsObsolete { get; set; }

        // Relacje
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public ICollection<Element> Elements { get; set; } = new List<Element>();
    }
}
