namespace VoltFlow.Service.Core.Models.ElementGroup.Request
{
    public class CreateElementGroupRequest
    {
        public string Name { get;}
        public int CategoryId { get; }
        public CreateElementGroupRequest(string name, int categoryId)
        {
            Name = name;
            CategoryId = categoryId;
        }
    }
}
