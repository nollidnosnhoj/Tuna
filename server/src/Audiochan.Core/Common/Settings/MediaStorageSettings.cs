using System.Collections.Generic;

namespace Audiochan.Core.Common.Settings
{
    public record MediaStorageSettings
    {
        public record StorageSettings
        {
            public string Bucket { get; init; }
            public string Container { get; init; }
            public List<string> ValidContentTypes { get; init; }
            public long MaximumFileSize { get; init; }
        }

        public StorageSettings Audio { get; init; }
        public StorageSettings Image { get; init; }
    }
}