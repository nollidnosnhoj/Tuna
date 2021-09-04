using System.Collections.Generic;

namespace Audiochan.Core.Playlists.RemoveAudiosFromPlaylist
{
    public class RemoveAudiosFromPlaylistRequest
    {
        public List<long> PlaylistAudioIds { get; set; } = new();
    }
}