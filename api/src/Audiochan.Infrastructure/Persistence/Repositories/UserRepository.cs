using Ardalis.Specification;
using Audiochan.Core.Entities;
using Audiochan.Core.Persistence;
using JetBrains.Annotations;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    internal class UserRepository : EfRepository<User>, IUserRepository
    {
        public UserRepository([NotNull] ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public UserRepository([NotNull] ApplicationDbContext dbContext, [NotNull] ISpecificationEvaluator specificationEvaluator) 
            : base(dbContext, specificationEvaluator)
        {
        }
    }
}