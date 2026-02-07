using VoltFlow.Service.Core.Models.Element;

namespace VoltFlow.Service.Core.Models.Common
{
    public class PagedResultDTO<T>
    {
        public List<T> Relusts { get; set; } = new();
        public int TotalCount { get; set; }
    }

}
