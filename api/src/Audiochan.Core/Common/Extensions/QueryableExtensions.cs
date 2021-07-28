using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Common.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<List<Tag>> GetAppropriateTags(this DbSet<Tag> dbSet, IEnumerable<string> tags, 
            CancellationToken cancellationToken)
        {
            var taggifyTags = tags.FormatTags();

            var tagEntities = await dbSet
                .Where(tag => taggifyTags.Contains(tag.Name))
                .ToListAsync(cancellationToken);

            foreach (var tag in taggifyTags.Where(tag => tagEntities.All(t => t.Name != tag)))
            {
                tagEntities.Add(new Tag {Name = tag});
            }

            return tagEntities;
        }

        public static IQueryable<Audio> FilterCursor(this IQueryable<Audio> queryable, string? cursor)
        {
            if (cursor is null)
                return queryable;

            var (id, since) = CursorHelpers.Decode(cursor);
            
            if (id is not null && since is not null)
            {
                queryable = queryable
                    .Where(a => a.Created < since || a.Created == since && a.Id.CompareTo(id) < 0);
            }

            return queryable;
        }
    }
}