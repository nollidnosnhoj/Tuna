using Ardalis.Specification;
using Audiochan.Core.Entities;
using Audiochan.Core.Repositories;
using JetBrains.Annotations;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    internal class FavoriteAudioRepository : EfRepository<FavoriteAudio>, IFavoriteAudioRepository
    {
        public FavoriteAudioRepository([NotNull] ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public FavoriteAudioRepository([NotNull] ApplicationDbContext dbContext, [NotNull] ISpecificationEvaluator specificationEvaluator) 
            : base(dbContext, specificationEvaluator)
        {
        }
    }
}