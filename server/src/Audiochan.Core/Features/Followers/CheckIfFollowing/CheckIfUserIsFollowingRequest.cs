using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Followers.CheckIfFollowing
{
    public record CheckIfUserIsFollowingRequest(string UserId, string Username) : IRequest<bool>
    {
    }

    public class CheckIfUserIsFollowingRequestHandler : IRequestHandler<CheckIfUserIsFollowingRequest, bool>
    {
        private readonly IApplicationDbContext _dbContext;

        public CheckIfUserIsFollowingRequestHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(CheckIfUserIsFollowingRequest request, CancellationToken cancellationToken)
        {
            return await _dbContext.FollowedUsers
                .AsNoTracking()
                .Include(u => u.Target)
                .AnyAsync(u => u.ObserverId == request.UserId
                               && u.Target.UserName == request.Username.Trim().ToLower(), cancellationToken);
        }
    }
}