using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<TEntity> SetTracking<TEntity>(this IQueryable<TEntity> queryable, bool isTracking = true)
            where TEntity : class
        {
            return isTracking ? queryable : queryable.AsNoTracking();
        }
    }
}