using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Audios;
using Audiochan.Core.Playlists.GetPlaylistAudios;
using Audiochan.Core.Users.GetUserFavoriteAudios;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Common.Interfaces.Persistence
{
    public interface IAudioRepository : IEntityRepository<Audio>
    {
        Task<List<PlaylistAudioDto>> GetPlaylistAudios(GetPlaylistAudiosQuery query, CancellationToken cancellationToken = default);
        Task<List<AudioDto>> GetUserFavoriteAudios(GetUserFavoriteAudiosQuery query,
            CancellationToken cancellationToken = default);
    }
}