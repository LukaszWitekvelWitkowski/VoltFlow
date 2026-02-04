namespace VoltFlow.Service.Core.Entities
{
    public class Category
    {
        public int IdCategory { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsObsolete { get; set; }

        public ICollection<ElementGroup> ElementsGroups { get; set; } = new List<ElementGroup>();
    }
}