using Ardalis.Specification;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Entities;
using JetBrains.Annotations;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class UserRepository : EfRepository<User>, IUserRepository
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