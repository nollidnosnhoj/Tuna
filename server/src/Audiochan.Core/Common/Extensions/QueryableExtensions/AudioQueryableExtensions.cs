using System.Linq;
using Audiochan.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Common.Extensions.QueryableExtensions
{
    public static class AudioQueryableExtensions
    {
        public static IQueryable<Audio> BaseListQueryable(this IQueryable<Audio> dbSet, string currentUserId = "")
        {
            return dbSet
                .AsNoTracking()
                .Include(a => a.Tags)
                .Include(a => a.User)
                .Where(a => a.UserId == currentUserId || a.IsPublic);
        }

        public static IQueryable<Audio> BaseDetailQueryable(this IQueryable<Audio> dbSet, string privateKey = "",
            string currentUserId = "")
        {
            return dbSet
                .AsNoTracking()
                .Include(a => a.Tags)
                .Include(a => a.User)
                .Where(a => a.UserId == currentUserId || a.IsPublic || a.PrivateKey == privateKey && !a.IsPublic);
        }
    }
}