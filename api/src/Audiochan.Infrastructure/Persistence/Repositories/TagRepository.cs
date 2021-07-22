using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Entities;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class TagRepository : EfRepository<Tag>, ITagRepository
    {
        public async Task<List<Tag>> GetAppropriateTags(List<string> tags, CancellationToken cancellationToken = default)
        {
            var taggifyTags = tags.FormatTags();

            var tagEntities = await DbSet
                .Where(tag => taggifyTags.Contains(tag.Name))
                .ToListAsync(cancellationToken);

            foreach (var tag in taggifyTags.Where(tag => tagEntities.All(t => t.Name != tag)))
            {
                tagEntities.Add(new Tag {Name = tag});
            }

            return tagEntities;
        }

        public TagRepository(ApplicationDbContext dbContext, ICurrentUserService currentUserService) 
            : base(dbContext, currentUserService)
        {
        }
    }
}