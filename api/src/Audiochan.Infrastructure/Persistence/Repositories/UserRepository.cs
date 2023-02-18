using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Persistence;
using Audiochan.Core.Persistence.Repositories;
using Audiochan.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class UserRepository : EfRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<User?> GetUserWithLogin(string login, CancellationToken cancellationToken = default)
        {
            return await Queryable
                .Where(u => u.UserName == login || u.Email == login)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> LoadUserWithFollowers(long targetId, long observerId, CancellationToken cancellationToken = default)
        {
            IQueryable<User> queryable = Queryable;
            
            if (observerId > 0)
            {
                queryable = queryable.Include(a =>
                    a.Followers.Where(fa => fa.ObserverId == observerId));
            }
            else
            {
                queryable = queryable.Include(a => a.Followers);
            }

            return await queryable.SingleOrDefaultAsync(a => a.Id == targetId, cancellationToken);
        }

        public async Task<bool> CheckIfUsernameExists(string userName, CancellationToken cancellationToken = default)
        {
            return await Queryable.AnyAsync(u => u.UserName == userName, cancellationToken);
        }

        public async Task<bool> CheckIfEmailExists(string email, CancellationToken cancellationToken = default)
        {
            return await Queryable.AnyAsync(u => u.Email == email, cancellationToken);
        }
    }
}