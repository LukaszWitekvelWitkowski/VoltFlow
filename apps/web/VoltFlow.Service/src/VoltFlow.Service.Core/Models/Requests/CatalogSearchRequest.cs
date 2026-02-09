using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Core.Models.Requests
{
    public class CatalogSearchRequest : PaginationParams
    {
        public string? CategoryName { get; }
        public int? CategoryId { get; }
        public string? ElementGroupName { get; }
        public int? ElementGroupId { get; }
        public string? ElementName { get; }
        public int? ElementId { get; }
    }
}
