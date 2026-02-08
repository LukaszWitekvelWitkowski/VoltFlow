namespace VoltFlow.Service.Core.Models.Element.Request
{
    public class UpdateElementRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsObsolete { get; set; }
        public int ElementGroupId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
