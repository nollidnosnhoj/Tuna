using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using Audiochan.Infrastructure.Persistence.Repositories.Abstractions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class UserRepository : EfRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<long[]> GetObserverFollowingIds(long observerId, CancellationToken ct = default)
        {
            return await DbSet
                .AsNoTracking()
                .Where(user => user.Id == observerId)
                .SelectMany(u => u.Followings.Select(f => f.TargetId))
                .ToArrayAsync(ct);
        }
    }
}