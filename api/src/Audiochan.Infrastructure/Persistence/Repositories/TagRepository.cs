using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public TagRepository(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Tag>> GetListAsync(IEnumerable<string> tags,
            CancellationToken cancellationToken = default)
        {
            var taggifyTags = tags.FormatTags();

            var tagEntities = await _dbContext.Tags
                .Where(tag => taggifyTags.Contains(tag.Name))
                .ToListAsync(cancellationToken);

            foreach (var tag in taggifyTags.Where(tag => tagEntities.All(t => t.Name != tag)))
            {
                tagEntities.Add(new Tag {Name = tag});
            }

            return tagEntities;
        }
    }
}