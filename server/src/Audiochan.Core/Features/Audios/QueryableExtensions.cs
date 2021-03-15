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
                .Include(a => a.Favorited)
                .Include(a => a.User)
                .Include(a => a.Genre)
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

        public static IQueryable<Audio> FilterByGenre(this IQueryable<Audio> queryable, string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                long genreId = 0;

                if (long.TryParse(input, out var parsedId))
                    genreId = parsedId;

                queryable = queryable.Where(a => a.GenreId == genreId || a.Genre.Slug == input.Trim().ToLower());
            }

            return queryable;
        }

        public static IQueryable<Audio> Sort(this IQueryable<Audio> queryable, string sort)
        {
            return (sort?.ToLower() ?? "") switch
            {
                "favorites" => queryable.OrderByDescending(a => a.Favorited.Count)
                    .ThenByDescending(a => a.Created),
                "latest" => queryable.OrderByDescending(a => a.Created),
                _ => queryable.OrderByDescending(a => a.Created)
            };
        }
    }
}