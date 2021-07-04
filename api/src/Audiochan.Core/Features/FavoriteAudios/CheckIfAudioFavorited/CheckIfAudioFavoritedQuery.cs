using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.FavoriteAudios.CheckIfAudioFavorited
{
    public record CheckIfAudioFavoritedQuery(Guid AudioId, string UserId) : IRequest<bool>
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
            return await _unitOfWork.Audios.CheckIfFavoriteAudioExists(query.AudioId, query.UserId, cancellationToken);
        }
    }
}