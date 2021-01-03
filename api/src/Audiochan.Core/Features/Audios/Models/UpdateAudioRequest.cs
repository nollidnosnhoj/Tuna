using System.Collections.Generic;

namespace Audiochan.Core.Features.Audios.Models
{
    public class UpdateAudioRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? IsPublic { get; set; }
        public List<string?> Tags { get; set; } = new();
    }
}