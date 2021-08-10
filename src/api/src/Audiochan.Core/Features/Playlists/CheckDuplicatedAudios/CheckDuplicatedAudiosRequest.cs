using System;
using System.Collections.Generic;

namespace Audiochan.Core.Features.Playlists.CheckDuplicatedAudios
{
    public record CheckDuplicatedAudiosRequest
    {
        public List<long> AudioIds { get; set; } = new();
    }
}