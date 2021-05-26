using System;
using System.Linq;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.GetAudioList;

namespace Audiochan.Core.Common.Extensions.QueryableExtensions
{
    public static class AudioQueryableExtensions
    {
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

        private static IQueryable<Audio> FilterByTags(this IQueryable<Audio> queryable, string[] tags)
        {
            return tags.Length > 0
                ? queryable.Where(a => a.Tags.Any(t => tags.Contains(t.Name)))
                : queryable;
        }
    }
}