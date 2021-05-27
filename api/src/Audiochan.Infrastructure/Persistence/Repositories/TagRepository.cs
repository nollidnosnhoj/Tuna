using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Entities;
using Audiochan.Core.Extensions;
using Audiochan.Core.Repositories;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    internal class TagRepository : EfRepository<Tag>, ITagRepository
    {
        public TagRepository([NotNull] ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public TagRepository([NotNull] ApplicationDbContext dbContext, [NotNull] ISpecificationEvaluator specificationEvaluator) 
            : base(dbContext, specificationEvaluator)
        {
        }

        public async Task<List<Tag>> GetAppropriateTags(List<string> tags,
            CancellationToken cancellationToken = default)
        {
            var taggifyTags = tags.FormatTags();

            var tagEntities =
                await GetListAsync(tag => tags.Contains(tag.Name), cancellationToken: cancellationToken);

            foreach (var tag in taggifyTags.Where(tag => tagEntities.All(t => t.Name != tag)))
            {
                tagEntities.Add(new Tag {Name = tag});
            }

            return tagEntities;
        }
    }
}