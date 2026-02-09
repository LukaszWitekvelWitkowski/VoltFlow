using VoltFlow.Service.Core.Models.Element;

namespace VoltFlow.Service.Core.Models.Common
{
    public class PagedResultDTO<T> : PaginationParams
    {
        public PagedResultDTO(List<T> relusts, int totalCount, int number, int size)
        {
            PageNumber = number;
            PageSize = size;
            Results = relusts;
            TotalCount = totalCount;

        }

        public List<T> Results { get; set; } = new();
        public int TotalCount { get; set; }
    }

}
