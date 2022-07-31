using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.CQRS;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Users.Queries
{
    public record CheckIfUserIsFollowingQuery(long ObserverId, long TargetId) : IQueryRequest<bool>
    {
    }

    public class CheckIfUserIsFollowingQueryHandler : IRequestHandler<CheckIfUserIsFollowingQuery, bool>
    {
        private readonly ApplicationDbContext _dbContext;

        public CheckIfUserIsFollowingQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(CheckIfUserIsFollowingQuery query, CancellationToken cancellationToken)
        {
            return await _dbContext.FollowedUsers
                .AnyAsync(fu => fu.ObserverId == query.ObserverId && fu.TargetId == query.TargetId,
                    cancellationToken);
        }
    }
}