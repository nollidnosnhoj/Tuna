using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Common.Extensions
{
    public static class PagedListExtensions
    {
        private const int DefaultPageSize = 30;

        public static async Task<OffsetPagedListDto<TResponse>> OffsetPaginateAsync<TResponse>(
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

            var list = await queryable.Take(limit).ToListAsync(cancellationToken);

            int? nextOffset = list.Count == limit
                ? offset + limit
                : null;

            return new OffsetPagedListDto<TResponse>(list, nextOffset, limit);
        }

        public static async Task<OffsetPagedListDto<TResponse>> OffsetPaginateAsync<TResponse>(this IQueryable<TResponse> queryable
            , IHasOffsetPage paginationQuery
            , CancellationToken cancellationToken = default)
        {
            return await queryable.OffsetPaginateAsync(paginationQuery.Offset, paginationQuery.Size, cancellationToken);
        }
        
        public static async Task<PagedListDto<TResponse>> PaginateAsync<TResponse>(
            this IQueryable<TResponse> queryable,
            int page = 1,
            int limit = DefaultPageSize,
            CancellationToken cancellationToken = default)
        {
            if (page == default) page = 1;
            if (limit == default) limit = DefaultPageSize;
            var count = await queryable.CountAsync(cancellationToken);
            var pageNumber = Math.Max(1, page);
            var pageLimit = BoundLimit(limit);
            var list = await queryable
                .Skip((pageNumber - 1) * pageLimit)
                .Take(pageLimit)
                .ToListAsync(cancellationToken);
            return new PagedListDto<TResponse>(list, count, page, limit);
        }

        public static async Task<PagedListDto<TResponse>> PaginateAsync<TResponse>(this IQueryable<TResponse> queryable
            , IHasPage paginationQuery
            , CancellationToken cancellationToken = default)
        {
            return await queryable.PaginateAsync(paginationQuery.Page, paginationQuery.Size, cancellationToken);
        }

        private static int BoundLimit(int limit)
        {
            return Math.Min(DefaultPageSize, Math.Max(1, limit));
        }
    }
}