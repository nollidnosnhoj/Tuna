using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tuna.Application.Entities;
using Tuna.Application.Persistence;
using Tuna.Application.Persistence.Repositories;

namespace Tuna.Infrastructure.Persistence.Repositories;

public class UserRepository : EfRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<User?> LoadUserWithFollowers(long targetId, long observerId,
        CancellationToken cancellationToken = default)
    {
        var queryable = Queryable;

        if (observerId > 0)
            queryable = queryable.Include(a =>
                a.Followers.Where(fa => fa.ObserverId == observerId));
        else
            queryable = queryable.Include(a => a.Followers);

        return await queryable.SingleOrDefaultAsync(a => a.Id == targetId, cancellationToken);
    }
}