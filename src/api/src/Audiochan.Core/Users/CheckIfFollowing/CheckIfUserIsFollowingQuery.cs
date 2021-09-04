using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces.Persistence;
using MediatR;

namespace Audiochan.Core.Users.CheckIfFollowing
{
    public record CheckIfUserIsFollowingQuery(long ObserverId, long TargetId) : IRequest<bool>
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