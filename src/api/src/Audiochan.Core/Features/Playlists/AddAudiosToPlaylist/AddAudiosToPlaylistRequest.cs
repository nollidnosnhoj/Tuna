using System.Collections.Generic;

namespace Audiochan.Core.Features.Playlists.AddAudiosToPlaylist
{
    public record AddAudiosToPlaylistRequest
    {
        public List<long> AudioIds { get; set; } = new();
    }
}