using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Abstractions;
using Audiochan.Core.Entities.Enums;
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

        public static IQueryable<TEntity> FilterVisibility<TEntity>(this IQueryable<TEntity> queryable, 
            long currentUserId, FilterVisibilityMode mode) where TEntity : class, IHasVisibility
        {
            return mode switch
            {
                FilterVisibilityMode.Unlisted => queryable.Where(x =>
                    x.UserId == currentUserId || x.Visibility != Visibility.Private),
                _ => queryable.Where(x => x.UserId == currentUserId || x.Visibility == Visibility.Public)
            };
        }
    }
}