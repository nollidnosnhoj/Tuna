using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Audiochan.Core.Features.Audios.Models
{
    public record UploadAudioRequest
    {
        public IFormFile File { get; init; } = null!;
        public string? Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public bool? IsPublic { get; init; }
        public List<string?> Tags { get; init; } = new();
    }
}