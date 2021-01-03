using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Audiochan.Core.Features.Audios.Models
{
    public class UploadAudioRequest
    {
        public IFormFile File { get; set; } = null!;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? IsPublic { get; set; }
        public List<string?> Tags { get; set; } = new();
    }
}