using System;
using System.Collections.Generic;

namespace Audiochan.Core.Features.Playlists.CheckDuplicatedAudios
{
    public record CheckDuplicatedAudiosRequest
    {
        public List<Guid> AudioIds { get; set; } = new();
    }
}