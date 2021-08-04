using System;
using System.Collections.Generic;

namespace Audiochan.Core.Features.Playlists.RemoveAudiosFromPlaylist
{
    public class RemoveAudiosFromPlaylistRequest
    {
        public List<Guid> PlaylistAudioIds { get; set; } = new();
    }
}