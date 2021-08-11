using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Common.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<List<Tag>> GetAppropriateTags(this DbSet<Tag> dbSet, IEnumerable<string> tags, 
            CancellationToken cancellationToken)
        {
            var tagEntities = await dbSet
                .Where(tag => tags.Contains(tag.Name))
                .ToListAsync(cancellationToken);

            foreach (var tag in tags.Where(tag => tagEntities.All(t => t.Name != tag)))
            {
                tagEntities.Add(new Tag {Name = tag});
            }

            return tagEntities;
        }

        public static IQueryable<Audio> FilterCursor(this IQueryable<Audio> queryable, long? cursor)
        {
            if (cursor is null)
                return queryable;
            
            queryable = queryable.Where(a => a.Id < cursor);

            return queryable;
        }
    }
}