using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Playlists;
using Audiochan.Core.Users.GetUserFavoritePlaylists;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Common.Interfaces.Persistence
{
    public interface IPlaylistRepository : IEntityRepository<Playlist>
    {
        Task<List<PlaylistDto>> GetUserFavoritePlaylists(GetUserFavoritePlaylistsQuery query,
            CancellationToken ct = default);
    }
}