using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Followers.SetFollow
{
    public record SetFollowRequest(string UserId, string Username, bool IsFollowing) : IRequest<IResult<bool>>
    {
    }

    public class SetFollowRequestHandler : IRequestHandler<SetFollowRequest, IResult<bool>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IDateTimeProvider _dateTimeProvider;

        public SetFollowRequestHandler(IApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider)
        {
            _dbContext = dbContext;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IResult<bool>> Handle(SetFollowRequest request, CancellationToken cancellationToken)
        {
            if (!await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Id == request.UserId, cancellationToken))
                return Result<bool>.Fail(ResultError.Unauthorized);

            var target = await _dbContext.Users
                .Include(u => u.Followers)
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(u => u.UserName == request.Username.Trim().ToLower(), cancellationToken);

            if (target == null)
                return Result<bool>.Fail(ResultError.NotFound);

            if (target.Id == request.UserId)
                return Result<bool>.Fail(ResultError.Forbidden);

            var isFollowed = request.IsFollowing
                ? await Follow(target, request.UserId, cancellationToken)
                : await Unfollow(target, request.UserId, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(isFollowed);
        }

        private Task<bool> Follow(User target, string observerId, CancellationToken cancellationToken = default)
        {
            var follower = target.Followers.FirstOrDefault(f => f.ObserverId == observerId);

            if (follower is null)
            {
                follower = new FollowedUser
                {
                    TargetId = target.Id,
                    ObserverId = observerId,
                    FollowedDate = _dateTimeProvider.Now
                };
                
                target.Followers.Add(follower);
            }
            else if (follower.UnfollowedDate is not null)
            {
                follower.FollowedDate = _dateTimeProvider.Now;
                follower.UnfollowedDate = null;
            }
            
            return Task.FromResult(true);
        }

        private Task<bool> Unfollow(User target, string observerId, CancellationToken cancellationToken = default)
        {
            var follower = target.Followers.FirstOrDefault(f => f.ObserverId == observerId);

            if (follower is not null)
            {
                follower.UnfollowedDate = _dateTimeProvider.Now;
            }

            return Task.FromResult(false);
        }
    }
}