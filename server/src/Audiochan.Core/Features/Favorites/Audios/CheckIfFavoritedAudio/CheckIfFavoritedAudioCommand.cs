using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Favorites.Audios.CheckIfFavoritedAudio
{
    public record CheckIfFavoritedAudioCommand(string UserId, long AudioId) : IRequest<bool>
    {
    }

    public class CheckIfFavoritedAudioCommandHandler : IRequestHandler<CheckIfFavoritedAudioCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;

        public CheckIfFavoritedAudioCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(CheckIfFavoritedAudioCommand request, CancellationToken cancellationToken)
        {
            return await _dbContext.FavoriteAudios.AsNoTracking()
                .AnyAsync(fa => fa.AudioId == request.AudioId && fa.UserId == request.UserId, cancellationToken);
        }
    }
}