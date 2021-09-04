using System.Collections.Generic;

namespace Audiochan.Core.Playlists.UpdatePlaylistDetails
{
    public record UpdatePlaylistDetailsRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public List<string>? Tags { get; init; }
    }
}