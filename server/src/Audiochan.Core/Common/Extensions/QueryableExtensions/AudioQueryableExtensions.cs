using System.Linq;
using Audiochan.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Common.Extensions.QueryableExtensions
{
    public static class AudioQueryableExtensions
    {
        public static IQueryable<Audio> DefaultQueryable(this IQueryable<Audio> dbSet)
        {
            return dbSet
                .AsNoTracking()
                .Include(a => a.Tags)
                .Include(a => a.User)
                .Where(a => a.IsPublish);
        }

        public static IQueryable<Audio> ExcludePrivateAudios(this IQueryable<Audio> queryable, string currentUserId = "")
        {
            return string.IsNullOrEmpty(currentUserId)
                ? queryable.Where(a => a.IsPublic)
                : queryable.Where(a => a.IsPublic || a.UserId == currentUserId);
        }
    }
}