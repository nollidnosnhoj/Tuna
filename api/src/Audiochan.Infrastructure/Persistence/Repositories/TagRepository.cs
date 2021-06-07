using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Entities;
using Audiochan.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    internal class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TagRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Tag>> GetAppropriateTags(List<string> tags, CancellationToken cancellationToken = default)
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