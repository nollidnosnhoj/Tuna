using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Application.Features.Users.Queries.CheckIfAudioFavorited
{
    public record CheckIfAudioFavoritedQuery(long AudioId, long UserId) : IQueryRequest<bool>
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