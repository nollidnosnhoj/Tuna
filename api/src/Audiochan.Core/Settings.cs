using System.Collections.Generic;
using Audiochan.Common.Extensions;

namespace Audiochan.Core
{
    public class AuthenticationSettings
    {
        public string DiscordClientId { get; set; } = null!;
        public string DiscordClientSecret { get; set; } = null!;
    }

    public record MediaStorageSettings
    {
        public AudioStorageSettings Audio { get; init; } = null!;
        public PictureStorageSettings Image { get; init; } = null!;
    }

    public record AudioStorageSettings
    {
        public string Bucket { get; init; } = string.Empty;
        public string TempBucket { get; init; } = string.Empty;
        public List<string> ValidContentTypes { get; init; } = null!;
        public long MaximumFileSize { get; init; }
    }

    public record PictureStorageSettings
    {
        public string Bucket { get; init; } = string.Empty;
        public List<string> ValidContentTypes { get; init; } = null!;
        public long MaximumFileSize { get; init; }
    }
}