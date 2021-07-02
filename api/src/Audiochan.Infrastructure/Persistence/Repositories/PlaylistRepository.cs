using Audiochan.Core.Entities;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using AutoMapper;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class PlaylistRepository : EfRepository<Playlist>, IPlaylistRepository
    {
        public PlaylistRepository(ApplicationDbContext dbContext, ICurrentUserService currentUserService, IMapper mapper) 
            : base(dbContext, currentUserService, mapper)
        {
        }
    }
}