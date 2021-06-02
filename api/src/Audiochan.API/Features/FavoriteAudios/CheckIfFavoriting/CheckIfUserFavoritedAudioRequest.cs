using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            return await _unitOfWork.FavoriteAudios
                .AsNoTracking()
                .Where(u => u.AudioId == request.AudioId && u.UserId == request.UserId)
                .AnyAsync(cancellationToken);
        }
    }
}