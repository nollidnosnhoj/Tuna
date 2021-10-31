using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces.Persistence;
using MediatR;

namespace Audiochan.Core.Users
{
    public record CheckIfAudioFavoritedQuery(long AudioId, long UserId) : IRequest<bool>
    {
    }
    
    public class CheckIfUserFavoritedAudioQueryHandler : IRequestHandler<CheckIfAudioFavoritedQuery, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckIfUserFavoritedAudioQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task<bool> Handle(CheckIfAudioFavoritedQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.FavoriteAudios
                .ExistsAsync(fa => fa.AudioId == query.AudioId && fa.UserId == query.UserId, cancellationToken);
        }
    }
}