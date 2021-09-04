using System.Collections.Generic;

namespace Audiochan.Core.Playlists.GetDuplicatedAudiosInPlaylist
{
    public record CheckDuplicatedAudiosRequest
    {
        public List<long> AudioIds { get; set; } = new();
    }
}