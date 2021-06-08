using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.FavoriteAudios.CheckIfFavoriting
{
    public record CheckIfUserFavoritedAudioQuery(Guid AudioId, string UserId) : IRequest<bool>
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
            return await _unitOfWork.FavoriteAudios
                .AsNoTracking()
                .Where(u => u.AudioId == query.AudioId && u.UserId == query.UserId)
                .AnyAsync(cancellationToken);
        }
    }
}