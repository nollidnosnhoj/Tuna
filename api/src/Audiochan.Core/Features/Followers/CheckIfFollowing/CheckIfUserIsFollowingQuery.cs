using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.Followers.CheckIfFollowing
{
    public record CheckIfUserIsFollowingQuery(string ObserverId, string TargetId) : IRequest<bool>
    {
    }

    public class CheckIfUserIsFollowingQueryHandler : IRequestHandler<CheckIfUserIsFollowingQuery, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckIfUserIsFollowingQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CheckIfUserIsFollowingQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.FollowedUsers
                .ExistsAsync(fu => fu.ObserverId == query.ObserverId && fu.TargetId == query.TargetId,
                    cancellationToken);
        }
    }
}