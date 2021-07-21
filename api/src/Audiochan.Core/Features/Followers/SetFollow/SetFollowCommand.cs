using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Features.Followers.SetFollow
{
    public record SetFollowCommand(string ObserverId, string TargetId, bool IsFollowing) : IRequest<Result<bool>>
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
                .LoadWithFollowers(command.TargetId, command.ObserverId, cancellationToken);

            if (target == null)
                return Result<bool>.NotFound<User>();

            if (target.Id == command.ObserverId)
                return Result<bool>.Forbidden();

            var isFollowed = command.IsFollowing
                ? await Follow(target, command.ObserverId, cancellationToken)
                : await Unfollow(target, command.ObserverId, cancellationToken);

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