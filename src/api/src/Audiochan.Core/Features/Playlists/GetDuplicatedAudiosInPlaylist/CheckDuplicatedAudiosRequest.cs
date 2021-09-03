using System.Collections.Generic;

namespace Audiochan.Core.Features.Playlists.GetDuplicatedAudiosInPlaylist
{
    public record CheckDuplicatedAudiosRequest
    {
        public List<long> AudioIds { get; set; } = new();
    }
}