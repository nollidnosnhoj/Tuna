using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Extensions
{
    public static class QueryableExtensions
    {
        private const int DefaultPageSize = 30;

        public static async Task<List<TResponse>> CursorPaginateAsync<TResponse, TKey>(
            this IQueryable<TResponse> queryable,
            TKey cursor,
            int size = DefaultPageSize,
            CancellationToken cancellationToken = default) 
                where TResponse : IHasId<TKey>
                where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            if (size == default) size = DefaultPageSize;
            if (cursor.CompareTo(default) > 0)
            {
                queryable = queryable.Where(x => x.Id.CompareTo(cursor) < 0);
            }

            return await queryable
                .Take(size)
                .ToListAsync(cancellationToken);
        }

        public static async Task<List<TResponse>> OffsetPaginateAsync<TResponse>(
            this IQueryable<TResponse> queryable,
            int offset = 0,
            int limit = DefaultPageSize,
            CancellationToken cancellationToken = default)
        {
            if (offset == default) offset = 0;
            if (limit == default) limit = DefaultPageSize;
            limit = BoundLimit(limit);
            queryable = offset > 0
                ? queryable.Skip(offset)
                : queryable;

            return await queryable.Take(limit).ToListAsync(cancellationToken);
        }

        public static async Task<List<TResponse>> PaginateAsync<TResponse>(
            this IQueryable<TResponse> queryable,
            int page = 1,
            int limit = DefaultPageSize,
            CancellationToken cancellationToken = default)
        {
            if (page == default) page = 1;
            if (limit == default) limit = DefaultPageSize;
            var pageNumber = Math.Max(1, page);
            var pageLimit = BoundLimit(limit);
            return await queryable
                .Skip((pageNumber - 1) * pageLimit)
                .Take(pageLimit)
                .ToListAsync(cancellationToken);
        }

        private static int BoundLimit(int limit)
        {
            return Math.Min(DefaultPageSize, Math.Max(1, limit));
        }
    }
}