using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.FavoriteAudios.CheckIfAudioFavorited
{
    public record CheckIfAudioFavoritedQuery(Guid AudioId, string UserId) : IRequest<bool>
    {
    }
    
    public class CheckIfUserFavoritedAudioQueryHandler : IRequestHandler<CheckIfAudioFavoritedQuery, bool>
    {
        private readonly ApplicationDbContext _unitOfWork;

        public CheckIfUserFavoritedAudioQueryHandler(ApplicationDbContext unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task<bool> Handle(CheckIfAudioFavoritedQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.FavoriteAudios
                .AnyAsync(fa => fa.AudioId == query.AudioId && fa.UserId == query.UserId, cancellationToken);
        }
    }
}