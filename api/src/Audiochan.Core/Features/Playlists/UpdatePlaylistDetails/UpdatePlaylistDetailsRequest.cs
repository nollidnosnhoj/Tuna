using Audiochan.Core.Entities.Enums;

namespace Audiochan.Core.Features.Playlists.UpdatePlaylistDetails
{
    public record UpdatePlaylistDetailsRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Visibility? Visibility { get; set; }
    }
}