namespace Audiochan.Common.Extensions
{
    public static class QueryableExtensions
    {
        private const int DefaultPageSize = 30;

        public static IQueryable<TResponse> OffsetPaginate<TResponse>(
            this IQueryable<TResponse> queryable,
            int offset = 0,
            int limit = DefaultPageSize)
        {
            if (offset == default) offset = 0;
            if (limit == default) limit = DefaultPageSize;
            limit = BoundLimit(limit);
            queryable = offset > 0
                ? queryable.Skip(offset)
                : queryable;

            return queryable.Take(limit);
        }

        public static IQueryable<TResponse> Paginate<TResponse>(
            this IQueryable<TResponse> queryable,
            int page = 1,
            int limit = DefaultPageSize)
        {
            if (page == default) page = 1;
            if (limit == default) limit = DefaultPageSize;
            var pageNumber = Math.Max(1, page);
            var pageLimit = BoundLimit(limit);
            return queryable
                .Skip((pageNumber - 1) * pageLimit)
                .Take(pageLimit);
        }

        private static int BoundLimit(int limit)
        {
            return Math.Min(DefaultPageSize, Math.Max(1, limit));
        }
    }
}