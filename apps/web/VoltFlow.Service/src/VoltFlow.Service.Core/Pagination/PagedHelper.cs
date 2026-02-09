using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Core.Pagination
{
    public static class PagedHelper
    {
        public static ServiceResponse<PagedResultDTO<T>> ToPagedResponse<T>(
            ServiceResponse<IEnumerable<T>> sourceResponse,
            string? filterValue,
            Func<T, string?> filterPropertySelector,
            int pageNumber,
            int pageSize)
        {

           // 1. Source Validation
            if (!sourceResponse._IsSuccess)
            {
                return ServiceResponse<PagedResultDTO<T>>.Failure(sourceResponse._Message, sourceResponse._StatusCode);
            }

            var query = sourceResponse._Data?.AsEnumerable() ?? Enumerable.Empty<T>();


            // 2. Dynamic filtering (Like)
            if (!string.IsNullOrWhiteSpace(filterValue))
            {
                query = query.Where(item =>
                {
                    var val = filterPropertySelector(item);
                    return val != null && val.Contains(filterValue, StringComparison.OrdinalIgnoreCase);
                });
            }

            // 3. Calculations
            int totalCount = query.Count();
            var items = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 4. Result
            var pagedData = new PagedResultDTO<T> (items, totalCount, pageNumber, pageSize);

            return ServiceResponse<PagedResultDTO<T>>.Result(pagedData);
        }
    }
}
