namespace VoltFlow.Service.Core.Models.Requests
{
    public class SearchRequest
    {
        public string? Name { get; set; }
        public int Number { get; set; } = 1;
        public int Size { get; set; } = 10;
    }

}