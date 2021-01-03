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
        public static async Task<List<T>> Paginate<T>(this IQueryable<T> queryable
            , int pageNumber
            , int pageSize
            , CancellationToken cancellationToken = default)
        {
            var page = pageNumber > 0 ? pageNumber : 1;
            var size = pageSize < 1 ? 1 : pageSize > 30 ? 30 : pageSize;
            return await queryable
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(cancellationToken);
        }
        
        public static async Task<List<T>> Paginate<T>(this IQueryable<T> queryable
            , PaginationQuery paginationQuery
            , CancellationToken cancellationToken = default)
        {
            return await queryable.Paginate(paginationQuery.Page, paginationQuery.Size, cancellationToken);
        }

        public static async Task<List<T>> Paginate<T>(this IQueryable<T> queryable,
            CancellationToken cancellationToken = default)
        {
            return await queryable.Paginate(1, 15, cancellationToken);
        }
    }
}