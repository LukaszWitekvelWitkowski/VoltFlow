namespace VoltFlow.Service.Core.Entities
{
    public class Element // 'Object' jest słowem kluczowym w C#, lepiej użyć ObjectEntity
    {
        public int IdElement { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsObsolete { get; set; }

        // Relacje
        public int ElementGroupId { get; set; }
        public ElementGroup ElementGroup { get; set; } = null!;
        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
    }
}
