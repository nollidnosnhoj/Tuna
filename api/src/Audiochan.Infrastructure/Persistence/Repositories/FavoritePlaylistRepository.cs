using Audiochan.Core.Entities;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class FavoritePlaylistRepository : EfRepository<FavoritePlaylist>, IFavoritePlaylistRepository
    {
        public FavoritePlaylistRepository(ApplicationDbContext dbContext, ICurrentUserService currentUserService) 
            : base(dbContext, currentUserService)
        {
        }
    }
}