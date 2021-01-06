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
        
        public static async Task<List<T>> Paginate<T>(this IQueryable<T> queryable, int page, int limit, 
            CancellationToken cancellationToken = default)
        {
            var pageNumber = Math.Max(1, page);
            var pageLimit = Math.Min(30, Math.Max(0, limit));
            return await queryable
                .Skip((pageNumber - 1) * pageLimit)
                .Take(pageLimit)
                .ToListAsync(cancellationToken);
        }
        
        public static async Task<List<T>> Paginate<T>(this IQueryable<T> queryable
            , PaginationQuery paginationQuery
            , CancellationToken cancellationToken = default)
        {
            return await queryable.Paginate(paginationQuery.Page, paginationQuery.Limit, cancellationToken);
        }

        public static async Task<List<T>> Paginate<T>(this IQueryable<T> queryable,
            CancellationToken cancellationToken = default)
        {
            return await queryable.Paginate(1, 15, cancellationToken);
        }
    }
}