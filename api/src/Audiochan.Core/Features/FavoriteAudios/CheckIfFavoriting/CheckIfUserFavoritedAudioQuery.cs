using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.FavoriteAudios.CheckIfFavoriting
{
    public record CheckIfUserFavoritedAudioQuery(long AudioId, string UserId) : IRequest<bool>
    {
    }
    
    public class CheckIfUserFavoritedAudioQueryHandler : IRequestHandler<CheckIfUserFavoritedAudioQuery, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckIfUserFavoritedAudioQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task<bool> Handle(CheckIfUserFavoritedAudioQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Audios.CheckIfFavoriteAudioExists(query.AudioId, query.UserId, cancellationToken);
        }
    }
}