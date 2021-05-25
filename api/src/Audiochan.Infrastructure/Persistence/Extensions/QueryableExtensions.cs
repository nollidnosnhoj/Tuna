using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> IgnoreGlobalQueryFilters<T>(this IQueryable<T> queryable, bool ignoreGlobalFilter) 
            where T : class
        {
            return ignoreGlobalFilter 
                ? queryable.IgnoreQueryFilters() 
                : queryable;
        }
    }
}