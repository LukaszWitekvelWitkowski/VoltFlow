using VoltFlow.Service.Core.Abstractions.Generic;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Core.Pagination
{
    public static class PagedHelper
    {
        public static PagedResultDTO<T> ToPagedResponse<T>(
            ICacheData<T> cache,
            string? filterValue,
            Func<T, string?> filterPropertySelector,
            int pageNumber,
            int pageSize) where T : class
        {
            if (cache?.Items == null) return new PagedResultDTO<T>(new List<T>(), 0, pageNumber, pageSize);

            var query = cache.Items.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(filterValue))
            {
                query = query.Where(item =>
                {
                    var val = filterPropertySelector(item);
                    return val != null && val.Contains(filterValue, StringComparison.OrdinalIgnoreCase);
                });
            }

            int totalCount = query.Count();
            var items = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResultDTO<T>(items, totalCount, pageNumber, pageSize);
        }
    }
 }
