using System;
using System.Collections.Generic;

namespace Audiochan.Core.Features.Playlists.AddAudiosToPlaylist
{
    public record AddAudiosToPlaylistRequest
    {
        public List<Guid> AudioIds { get; set; } = new();
    }
}