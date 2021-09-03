using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Features.Followers.SetFollow
{
    public record SetFollowCommand(long ObserverId, long TargetId, bool IsFollowing) : IRequest<Result<bool>>
    {
    }
    
    public sealed class LoadUserForFollowingSpecification : Specification<User>
    {
        public LoadUserForFollowingSpecification(long targetId, long observerId)
        {
            if (observerId > 0)
            {
                Query.Include(a =>
                    a.Followers.Where(fa => fa.ObserverId == observerId));
            }
            else
            {
                Query.Include(a => a.Followers);
            }

            Query.Where(a => a.Id == targetId);
        }
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
                .GetFirstAsync(new LoadUserForFollowingSpecification(command.TargetId, command.ObserverId), cancellationToken);

            if (target == null)
                return Result<bool>.NotFound<User>();

            if (target.Id == command.ObserverId)
                return Result<bool>.Forbidden();

            var isFollowed = command.IsFollowing
                ? Follow(target, command.ObserverId, cancellationToken)
                : Unfollow(target, command.ObserverId, cancellationToken);

            _unitOfWork.Users.Update(target);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(isFollowed);
        }

        private bool Follow(User target, long observerId, CancellationToken cancellationToken = default)
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
            
            return true;
        }

        private bool Unfollow(User target, long observerId, CancellationToken cancellationToken = default)
        {
            var follower = target.Followers.FirstOrDefault(f => f.ObserverId == observerId);

            if (follower is not null)
            {
                follower.UnfollowedDate = _dateTimeProvider.Now;
            }

            return false;
        }
    }
}