using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Core.Models.Requests
{
    public class CatalogSearchRequest : PaginationParams
    {
        public string? CategoryName { get; set; }
        public int? CategoryId { get; set; }
        public string? ElementGroupName { get; set; }
        public int? ElementGroupId { get; set; }
        public string? ElementName { get; set; }
        public int? ElementId { get; set; }

    }
}
