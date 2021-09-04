using System.Collections.Generic;

namespace Audiochan.Core.Playlists.AddAudiosToPlaylist
{
    public record AddAudiosToPlaylistRequest
    {
        public List<long> AudioIds { get; set; } = new();
    }
}