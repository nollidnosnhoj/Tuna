using System.Collections.Generic;
using Audiochan.Core.Entities.Enums;

namespace Audiochan.Core.Features.Audios.UpdateAudio
{
    public class UpdateAudioRequest
    {
        public string? Title { get; init; }
        public string? Description { get; init; }
        public Visibility? Visibility { get; init; }
        public List<string>? Tags { get; init; }
    }
}