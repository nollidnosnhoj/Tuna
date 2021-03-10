using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Followers.CheckIfFollowing
{
    public record CheckIfFollowingCommand(string UserId, string Username) : IRequest<bool>
    {
    }

    public class CheckIfFollowingCommandHandler : IRequestHandler<CheckIfFollowingCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;

        public CheckIfFollowingCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(CheckIfFollowingCommand request, CancellationToken cancellationToken)
        {
            return await _dbContext.FollowedUsers
                .AsNoTracking()
                .Include(u => u.Target)
                .AnyAsync(u => u.ObserverId == request.UserId
                               && u.Target.UserName == request.Username.Trim().ToLower(), cancellationToken);
        }
    }
}