using System.Linq;
using Audiochan.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios
{
    public static class QueryableExtensions
    {
        public static IQueryable<Audio> DefaultQueryable(this DbSet<Audio> dbSet, string currentUserId = "")
        {
            return dbSet
                .AsNoTracking()
                .Include(a => a.Tags)
                .Include(a => a.User)
                .Where(a => a.UserId == currentUserId || a.IsPublic);
        }

        public static IQueryable<Audio> FilterByTags(this IQueryable<Audio> queryable, string tags, string delimiter)
        {
            if (!string.IsNullOrWhiteSpace(tags))
            {
                var parsedTags = tags.Split(delimiter)
                    .Select(t => t.Trim().ToLower())
                    .ToArray();

                queryable = queryable.Where(a => a.Tags.Any(t => parsedTags.Contains(t.Id)));
            }

            return queryable;
        }
        
        public static IQueryable<Audio> Sort(this IQueryable<Audio> queryable, string sort)
        {
            return (sort?.ToLower() ?? "") switch
            {
                "latest" => queryable.OrderByDescending(a => a.Created),
                _ => queryable.OrderByDescending(a => a.Created)
            };
        }
    }
}