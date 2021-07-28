using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Followers.CheckIfFollowing
{
    public record CheckIfUserIsFollowingQuery(string ObserverId, string TargetId) : IRequest<bool>
    {
    }

    public class CheckIfUserIsFollowingQueryHandler : IRequestHandler<CheckIfUserIsFollowingQuery, bool>
    {
        private readonly ApplicationDbContext _unitOfWork;

        public CheckIfUserIsFollowingQueryHandler(ApplicationDbContext unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CheckIfUserIsFollowingQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.FollowedUsers
                .AnyAsync(fu => fu.ObserverId == query.ObserverId && fu.TargetId == query.TargetId,
                    cancellationToken);
        }
    }
}