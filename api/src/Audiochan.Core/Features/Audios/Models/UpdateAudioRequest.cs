using System.Collections.Generic;

namespace Audiochan.Core.Features.Audios.Models
{
    public record UpdateAudioRequest
    {
        public string? Title { get; init; }
        public string? Description { get; init; }
        public bool? IsPublic { get; init; }
        public bool? IsLoop { get; init; }
        public List<string?> Tags { get; init; } = new();
    }
}