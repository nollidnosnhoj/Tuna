using System.Linq;
using Audiochan.Core.Entities.Abstractions;
using Audiochan.Core.Entities.Enums;
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

        public static IQueryable<TEntity> WhereVisible<TEntity>(this IQueryable<TEntity> queryable,
            string? privateKey, string currentUserId) where TEntity : class, IVisible
        {
            return queryable
                .Where(x => x.Visibility == Visibility.Private && x.PrivateKey != privateKey &&
                            x.UserId != currentUserId);
        }
    }
}