using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using Audiochan.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Followers.SetFollow
{
    public record SetFollowCommand(long ObserverId, long TargetId, bool IsFollowing) : IRequest<Result<bool>>
    {
    }

    public class SetFollowCommandHandler : IRequestHandler<SetFollowCommand, Result<bool>>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ApplicationDbContext _unitOfWork;

        public SetFollowCommandHandler(IDateTimeProvider dateTimeProvider, ApplicationDbContext unitOfWork)
        {
            _dateTimeProvider = dateTimeProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(SetFollowCommand command, CancellationToken cancellationToken)
        {
            var queryable = _unitOfWork.Users
                .IgnoreQueryFilters()
                .Where(u => u.Id == command.TargetId);

            queryable = UserHelpers.IsValidId(command.ObserverId)
                ? queryable.Include(u => u.Followers)
                : queryable.Include(u => u.Followers.Where(f => f.ObserverId == command.ObserverId));

            var target = await queryable.SingleOrDefaultAsync(cancellationToken);

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

        private Task<bool> Follow(User target, long observerId, CancellationToken cancellationToken = default)
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

        private Task<bool> Unfollow(User target, long observerId, CancellationToken cancellationToken = default)
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