using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Models.Interfaces;
using Audiochan.Core.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Extensions
{
    public static class PagedListExtensions
    {
        const int DefaultPageNumber = 1;
        private const int DefaultPageSize = 30;
        
        public static async Task<PagedList<TResponse>> PaginateAsync<TResponse>(
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
            return new PagedList<TResponse>(list, count, page, limit);
        }

        public static async Task<PagedList<TResponse>> PaginateAsync<TResponse>(this IQueryable<TResponse> queryable
            , IHasPage paginationQuery
            , CancellationToken cancellationToken = default)
        {
            return await queryable.PaginateAsync(paginationQuery.Page, paginationQuery.Size, cancellationToken);
        }

        public static async Task<CursorList<TResponse, long?>> CursorPaginateAsync<TResponse>(
            this IQueryable<TResponse> queryable,
            IHasCursor request,
            CancellationToken cancellationToken = default) where TResponse : IBaseViewModel
        {
            queryable = queryable.OrderByDescending(x => x.Id);
            
            if (request.Cursor.HasValue)
                queryable = queryable.Where(x => x.Id < request.Cursor);
            
            var items = await queryable
                .Take(request.Size ?? 30)
                .ToListAsync(cancellationToken);

            var prev = items.FirstOrDefault()?.Id;
            var next = items.LastOrDefault()?.Id;

            return new CursorList<TResponse, long?>(items, prev, next);
        }
    }
}