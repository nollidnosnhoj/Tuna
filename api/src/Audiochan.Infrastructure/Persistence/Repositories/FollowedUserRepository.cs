using Ardalis.Specification;
using Audiochan.Core.Entities;
using Audiochan.Core.Persistence;
using JetBrains.Annotations;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    internal class FollowedUserRepository : EfRepository<FollowedUser>, IFollowedUserRepository
    {
        public FollowedUserRepository([NotNull] ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public FollowedUserRepository([NotNull] ApplicationDbContext dbContext, [NotNull] ISpecificationEvaluator specificationEvaluator) 
            : base(dbContext, specificationEvaluator)
        {
        }
    }
}