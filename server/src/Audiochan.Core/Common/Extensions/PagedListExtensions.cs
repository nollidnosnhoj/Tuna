using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Common.Extensions
{
    public static class PagedListExtensions
    {
        public static async Task<PagedList<TResponse>> PaginateAsync<TResponse>(
            this IQueryable<TResponse> queryable,
            int page,
            int limit,
            CancellationToken cancellationToken = default)
        {
            var count = await queryable.CountAsync(cancellationToken);
            var pageNumber = Math.Max(1, page);
            var pageLimit = Math.Max(0, Math.Min(limit, 30));
            var list = await queryable
                .Skip((pageNumber - 1) * pageLimit)
                .Take(pageLimit)
                .ToListAsync(cancellationToken);
            return new PagedList<TResponse>(list, count, page, limit);
        }

        public static async Task<PagedList<TResponse>> PaginateAsync<TResponse>(this IQueryable<TResponse> queryable
            , PaginationQueryRequest paginationQuery
            , CancellationToken cancellationToken = default)
        {
            return await queryable.PaginateAsync(paginationQuery.Page, paginationQuery.Size, cancellationToken);
        }

        public static async Task<PagedList<TResponse>> PaginateAsync<TResponse>(this IQueryable<TResponse> queryable,
            CancellationToken cancellationToken = default)
        {
            var paginationQuery = new PaginationQueryRequest();
            return await queryable.PaginateAsync(paginationQuery.Page, paginationQuery.Size, cancellationToken);
        }
    }
}