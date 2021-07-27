using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Audios.GetLatestAudios;
using Audiochan.Core.Features.Playlists.GetPlaylistAudios;
using Audiochan.Core.Features.Playlists.GetPlaylistDetail;

namespace Audiochan.Core.Repositories
{
    public interface IPlaylistRepository : IGenericRepository<Playlist>
    {
        Task<PlaylistDetailViewModel?> Get(Guid id,
            CancellationToken cancellationToken = default);

        Task<PagedListDto<AudioViewModel>> GetAudios(GetPlaylistAudiosQuery query,
            CancellationToken cancellationToken = default);

        Task<Playlist?> LoadWithAudios(Guid id, List<Guid>? audioIds,
            CancellationToken cancellationToken = default);

        Task<Playlist?> LoadForUpdating(Guid id, CancellationToken cancellationToken = default);

        Task<Playlist?> LoadWithFavorites(Guid id, string userId = "",
            CancellationToken cancellationToken = default);
    }
}