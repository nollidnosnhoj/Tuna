using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Common.Extensions
{
    public static class PagedListExtensions
    {
        private const int DefaultPageNumber = 1;
        private const int DefaultPageSize = 30;

        public static async Task<PagedListDto<TResponse>> PaginateAsync<TResponse>(
            this IQueryable<TResponse> queryable,
            int page = DefaultPageNumber,
            int limit = DefaultPageSize,
            CancellationToken cancellationToken = default)
        {
            if (page == default) page = DefaultPageNumber;
            if (limit == default) limit = DefaultPageSize;
            var count = await queryable.CountAsync(cancellationToken);
            var pageNumber = Math.Max(DefaultPageNumber, page);
            var pageLimit = Math.Max(0, Math.Min(limit, DefaultPageSize));
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
    }
}