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
    }
}