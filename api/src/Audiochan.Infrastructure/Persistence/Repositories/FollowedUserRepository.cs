using Ardalis.Specification;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Entities;
using JetBrains.Annotations;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class FollowedUserRepository : EfRepository<FollowedUser>, IFollowedUserRepository
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