using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Features.Playlists;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using Audiochan.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public interface IPlaylistRepository : IGenericRepository<Playlist>
    {
        Task<bool> CheckIfPlaylistHasAudio(long playlistId, long audioId,
            CancellationToken cancellationToken = default);
        Task<Playlist> LoadPlaylistForAudios(long playlistId, CancellationToken cancellationToken = default);
        Task<Playlist> LoadPlaylistForSetFavorite(long playlistId, CancellationToken cancellationToken = default);

        Task<PagedListDto<AudioViewModel>> GetPlaylistAudios(GetPlaylistAudiosQuery query, 
            CancellationToken cancellationToken = default);
    }
    
    public class PlaylistRepository : EfRepository<Playlist>, IPlaylistRepository
    {
        public PlaylistRepository(ApplicationDbContext dbContext, ICurrentUserService currentUserService) 
            : base(dbContext, currentUserService)
        {
        }

        public async Task<bool> CheckIfPlaylistHasAudio(long playlistId, long audioId, CancellationToken cancellationToken = default)
        {
            return await DbContext.PlaylistAudios
                .AnyAsync(x => x.AudioId == audioId && x.PlaylistId == playlistId, cancellationToken);
        }

        public async Task<Playlist> LoadPlaylistForAudios(long playlistId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(x => x.Id == playlistId)
                .Include(x => x.Audios)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<Playlist> LoadPlaylistForSetFavorite(long playlistId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(x => x.Id == playlistId)
                .Include(x => x.Favoriters)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<PagedListDto<AudioViewModel>> GetPlaylistAudios(GetPlaylistAudiosQuery query, 
            CancellationToken cancellationToken = default)
        {
            var currentUserId = CurrentUserService.GetUserId();
            return await DbSet
                .Include(x => x.Audios)
                .ThenInclude(x => x.Audio)
                .Where(x => x.Id == query.PlaylistId)
                .WhereVisible(query.PrivateKey, currentUserId)
                .SelectMany(x => x.Audios)
                .Select(x => x.Audio)
                .Where(x => x.Visibility == Visibility.Public || x.UserId == currentUserId)
                .ProjectToList()
                .PaginateAsync(query, cancellationToken);
        }
    }
}