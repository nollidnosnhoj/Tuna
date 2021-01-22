using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Common.Extensions
{
    public static class PagedListExtensions
    {
        
        public static async Task<PagedList<T>> Paginate<T>(this IQueryable<T> queryable, int page, int limit, 
            CancellationToken cancellationToken = default)
        {
            var count = await queryable.CountAsync(cancellationToken);
            var pageNumber = Math.Max(1, page);
            var pageLimit = Math.Min(30, Math.Max(0, limit));
            var list = await queryable
                .Skip((pageNumber - 1) * pageLimit)
                .Take(pageLimit)
                .ToListAsync(cancellationToken);
            return new PagedList<T>(list, count, page, limit);
        }
        
        public static async Task<PagedList<T>> Paginate<T>(this IQueryable<T> queryable
            , PaginationQuery paginationQuery
            , CancellationToken cancellationToken = default)
        {
            return await queryable.Paginate(paginationQuery.Page, paginationQuery.Size, cancellationToken);
        }

        public static async Task<PagedList<T>> Paginate<T>(this IQueryable<T> queryable,
            CancellationToken cancellationToken = default)
        {
            return await queryable.Paginate(1, 30, cancellationToken);
        }
    }
}