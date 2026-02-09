namespace VoltFlow.Service.Core.Models.Element.Request
{
    public class CreateElementRequest 
    {
        public CreateElementRequest(string name, int elementGroupId, string? description)
        {
            Name = name;
            ElementGroupId = elementGroupId;
            Description = description;
        }

        public string Name { get; set; }
        public int ElementGroupId { get; set; }
        public string? Description { get; set; }
    }
}
