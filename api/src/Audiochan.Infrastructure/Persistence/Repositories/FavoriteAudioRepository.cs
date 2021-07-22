using Audiochan.Core.Entities;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class FavoriteAudioRepository : EfRepository<FavoriteAudio>, IFavoriteAudioRepository
    {
        public FavoriteAudioRepository(ApplicationDbContext dbContext, ICurrentUserService currentUserService) 
            : base(dbContext, currentUserService)
        {
        }
    }
}