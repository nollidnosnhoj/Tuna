using Ardalis.Specification;
using Audiochan.Core.Entities;
using Audiochan.Core.Persistence;
using JetBrains.Annotations;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    internal class AudioRepository : EfRepository<Audio>, IAudioRepository
    {
        public AudioRepository([NotNull] ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public AudioRepository([NotNull] ApplicationDbContext dbContext,
            [NotNull] ISpecificationEvaluator specificationEvaluator)
            : base(dbContext, specificationEvaluator)
        {
        }
    }
}