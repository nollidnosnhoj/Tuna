using Audiochan.Core.Entities;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class FollowedUserRepository : EfRepository<FollowedUser>, IFollowedUserRepository
    {
        public FollowedUserRepository(ApplicationDbContext dbContext, ICurrentUserService currentUserService) 
            : base(dbContext, currentUserService)
        {
        }
    }
}