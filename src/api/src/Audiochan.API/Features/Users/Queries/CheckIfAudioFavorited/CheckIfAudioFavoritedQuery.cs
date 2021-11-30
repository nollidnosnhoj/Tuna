using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Users.Queries
{
    public record CheckIfAudioFavoritedQuery(long AudioId, long UserId) : IRequest<bool>
    {
    }
    
    public class CheckIfUserFavoritedAudioQueryHandler : IRequestHandler<CheckIfAudioFavoritedQuery, bool>
    {
        private readonly ApplicationDbContext _dbContext;

        public CheckIfUserFavoritedAudioQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<bool> Handle(CheckIfAudioFavoritedQuery query, CancellationToken cancellationToken)
        {
            return await _dbContext.FavoriteAudios
                .AnyAsync(fa => fa.AudioId == query.AudioId && fa.UserId == query.UserId, cancellationToken);
        }
    }
}