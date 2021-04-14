using System.Collections.Generic;
using System.Linq;
using Audiochan.Core.Entities;
using Audiochan.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Extensions.QueryableExtensions
{
    public static class AudioQueryableExtensions
    {
        public static IQueryable<Audio> BaseListQueryable(this IQueryable<Audio> dbSet, string currentUserId = "")
        {
            return dbSet
                .AsNoTracking()
                .Include(a => a.Tags)
                .Include(a => a.User)
                .Where(a => a.UserId == currentUserId || a.Visibility == Visibility.Public);
        }

        public static IQueryable<Audio> BaseDetailQueryable(this IQueryable<Audio> dbSet, string privateKey = "", string currentUserId = "")
        {
            return dbSet
                .AsNoTracking()
                .Include(a => a.Tags)
                .Include(a => a.User)
                .Where(a => a.UserId == currentUserId 
                            || a.Visibility != Visibility.Private
                            || (a.PrivateKey == privateKey 
                            && a.Visibility == Visibility.Private));
        }

        public static IQueryable<Audio> FilterByTags(this IQueryable<Audio> queryable, IEnumerable<string> tags)
        {
            return queryable.Where(a => a.Tags.Any(t => tags.Contains(t.Name)));
        }
    }
}