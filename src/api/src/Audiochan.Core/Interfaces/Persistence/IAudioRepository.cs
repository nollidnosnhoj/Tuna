using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Playlists.GetPlaylistAudios;
using Audiochan.Core.Features.Users.GetUserFavoriteAudios;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Interfaces.Persistence
{
    public interface IAudioRepository : IEntityRepository<Audio>
    {
        Task<List<AudioDto>> GetPlaylistAudios(GetPlaylistAudiosQuery query, CancellationToken cancellationToken = default);
        Task<List<AudioDto>> GetUserFavoriteAudios(GetUserFavoriteAudiosQuery query,
            CancellationToken cancellationToken = default);
    }
}