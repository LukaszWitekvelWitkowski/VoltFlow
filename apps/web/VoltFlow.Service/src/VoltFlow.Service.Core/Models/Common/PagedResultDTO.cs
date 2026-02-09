namespace VoltFlow.Service.Core.Models.Common
{
    public class PagedResultDTO<T> : PaginationParams
    {
        public PagedResultDTO() { }
        public PagedResultDTO(IEnumerable<T> relusts, int totalCount, int number, int size)
        {
            PageNumber = number;
            PageSize = size;
            Results = relusts;
            TotalCount = totalCount;

        }

        public IEnumerable<T> Results { get; set; } = Enumerable.Empty<T>();
        public int TotalCount { get; set; }
    }

}
