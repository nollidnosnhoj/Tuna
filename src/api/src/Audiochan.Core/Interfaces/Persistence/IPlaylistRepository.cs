using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Playlists;
using Audiochan.Core.Features.Users.GetUserFavoritePlaylists;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Interfaces.Persistence
{
    public interface IPlaylistRepository : IEntityRepository<Playlist>
    {
        Task<List<PlaylistDto>> GetUserFavoritePlaylists(GetUserFavoritePlaylistsQuery query,
            CancellationToken ct = default);
    }
}