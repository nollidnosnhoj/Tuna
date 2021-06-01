using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.API.Features.FavoriteAudios.CheckIfFavoriting
{
    public record CheckIfUserFavoritedAudioRequest(Guid AudioId, string UserId) : IRequest<bool>
    {
    }
    
    public class CheckIfUserFavoritedAudioRequestHandler : IRequestHandler<CheckIfUserFavoritedAudioRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckIfUserFavoritedAudioRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task<bool> Handle(CheckIfUserFavoritedAudioRequest request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.FavoriteAudios.ExistsAsync(
                new CheckIfUserFavoritedAudioSpecification(request.AudioId, request.UserId), 
                cancellationToken: cancellationToken);
        }
    }
}