using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces.Persistence;
using MediatR;

namespace Audiochan.Core.Artists.Queries
{
    public record CheckIfUserIsFollowingQuery(long UserId, long ArtistId) : IRequest<bool>
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
            return await _unitOfWork.FollowedArtists
                .ExistsAsync(fu => fu.ObserverId == query.UserId && fu.TargetId == query.ArtistId,
                    cancellationToken);
        }
    }
}