namespace VoltFlow.Service.Core.Models.Category.Request
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; }
        public CreateCategoryRequest(string name)
        {
            Name = name;
        }
    }
}
