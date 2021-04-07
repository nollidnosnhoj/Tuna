using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Common.Extensions
{
    public static class PagedListExtensions
    {
        public static async Task<PagedList<TResponse>> PaginateAsync<TResponse>(
            this IQueryable<TResponse> queryable,
            int page = 1,
            int limit = 30,
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