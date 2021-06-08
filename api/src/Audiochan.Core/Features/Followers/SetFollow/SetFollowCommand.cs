using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Followers.SetFollow
{
    public record SetFollowCommand(string UserId, string Username, bool IsFollowing) : IRequest<Result<bool>>
    {
    }

    public class SetFollowCommandHandler : IRequestHandler<SetFollowCommand, Result<bool>>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IUnitOfWork _unitOfWork;

        public SetFollowCommandHandler(IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork)
        {
            _dateTimeProvider = dateTimeProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(SetFollowCommand command, CancellationToken cancellationToken)
        {
            var target = await _unitOfWork.Users
                .Include(u => u.Followers)
                .IgnoreQueryFilters()
                .Where(u => u.UserName == command.Username.Trim().ToLower())
                .SingleOrDefaultAsync(cancellationToken);

            if (target == null)
                return Result<bool>.Fail(ResultError.NotFound);

            if (target.Id == command.UserId)
                return Result<bool>.Fail(ResultError.Forbidden);

            var isFollowed = command.IsFollowing
                ? await Follow(target, command.UserId, cancellationToken)
                : await Unfollow(target, command.UserId, cancellationToken);

            _unitOfWork.Users.Update(target);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

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