using System.Collections.Generic;

namespace Audiochan.Core.Common.Settings
{
    public record MediaStorageSettings
    {
        public record StorageSettings
        {
            public string Bucket { get; init; } = string.Empty;
            public string TempBucket { get; init; } = string.Empty;
            public string Container { get; init; } = null!;
            public List<string> ValidContentTypes { get; init; } = null!;
            public long MaximumFileSize { get; init; }
        }

        public StorageSettings Audio { get; init; } = null!;
        public StorageSettings Image { get; init; } = null!;
    }
}