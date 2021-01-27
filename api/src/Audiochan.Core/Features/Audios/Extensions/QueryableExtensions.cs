using System.Linq;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Features.Audios.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Filter only public audio uploads, unless the audio belongs to the current user, then display regardless
        /// </summary>
        public static IQueryable<Audio> FilterVisibility(this IQueryable<Audio> queryable, 
            string currentUserId)
        {
            return queryable
                .Where(a => a.UserId == currentUserId || a.IsPublic);
        }

        public static IQueryable<Audio> FilterByGenre(this IQueryable<Audio> queryable, string genreInput)
        {
            if (string.IsNullOrWhiteSpace(genreInput)) return queryable;

            long genreId = 0;

            if (long.TryParse(genreInput, out var parsedId))
                genreId = parsedId;

            return queryable.Where(a => a.GenreId == genreId || a.Genre.Slug == genreInput.Trim().ToLower());
        }
        
        public static IQueryable<Audio> FilterByTags(this IQueryable<Audio> queryable, string tags)
        {
            if (string.IsNullOrWhiteSpace(tags)) return queryable;
            
            var parsedTags = tags.Split(',')
                .Select(t => t.Trim().ToLower())
                .ToArray();
            
            return queryable.Where(a => a.Tags.Any(t => parsedTags.Contains(t.Id)));
        }
        
        public static IQueryable<Audio> Sort(this IQueryable<Audio> queryable, string orderBy)
        {
            return orderBy switch
            {
                "favorites" => 
                    queryable.OrderByDescending(a => a.Favorited.Count),
                _ => 
                    queryable.OrderByDescending(a => a.Created)
            };
        }
    }
}