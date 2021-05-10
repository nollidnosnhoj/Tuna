using System.Collections.Generic;
using System.Linq;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Common.Extensions.QueryableExtensions
{
    public static class AudioQueryableExtensions
    {
        public static IQueryable<Audio> IncludePublishAudios(this IQueryable<Audio> dbSet)
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

        public static IQueryable<Audio> FilterByTags(this IQueryable<Audio> queryable, string tags)
        {
            if (string.IsNullOrEmpty(tags)) return queryable;
            var parsedTags = tags.Split(',')
                .Select(t => t.Trim().ToLower())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToArray();
            return queryable.FilterByTags(parsedTags);
        }

        public static IQueryable<Audio> FilterByTags(this IQueryable<Audio> queryable, string[] tags)
        {
            return tags.Length > 0
                ? queryable.Where(a => a.Tags.Any(t => tags.Contains(t.Name)))
                : queryable;
        }
        
        public static IQueryable<Audio> FilterUsingCursor(this IQueryable<Audio> queryable, string cursor)
        {
            if (string.IsNullOrEmpty(cursor)) return queryable;
            var (since, id) = CursorHelpers.DecodeCursor(cursor);
            if (since.HasValue && !string.IsNullOrEmpty(id))
            {
                return queryable.Where(a => a.Created < since.GetValueOrDefault()
                                                 || (a.Created == since.GetValueOrDefault() 
                                                     && string.Compare(a.Id, id) < 0));
            }

            return queryable;
        }
    }
}