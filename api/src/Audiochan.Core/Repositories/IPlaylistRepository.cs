using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Features.Playlists.GetPlaylistAudios;
using Audiochan.Core.Features.Playlists.GetPlaylistDetail;

namespace Audiochan.Core.Repositories
{
    public interface IPlaylistRepository : IGenericRepository<Playlist>
    {
        Task<PlaylistDetailViewModel?> GetPlaylistDetail(Guid id,
            CancellationToken cancellationToken = default);

        Task<PagedListDto<AudioViewModel>> GetPlaylistAudios(GetPlaylistAudiosQuery query,
            CancellationToken cancellationToken = default);

        Task<Playlist?> LoadPlaylistForAudios(Guid id, List<Guid>? audioIds,
            CancellationToken cancellationToken = default);

        Task<Playlist?> LoadPlaylistForUpdate(Guid id, CancellationToken cancellationToken = default);

        Task<Playlist?> LoadPlaylistForFavoriting(Guid id, string userId = "",
            CancellationToken cancellationToken = default);
    }
}