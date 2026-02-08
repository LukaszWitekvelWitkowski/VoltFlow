namespace VoltFlow.Service.Core.Models.Element.Request
{
    public class CreateElementRequest 
    {
        public CreateElementRequest(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
