using System.Collections.Generic;

namespace Audiochan.Core.Common.Options
{
    public record AudiochanOptions
    {
        public record StorageOptions
        {
            public string Container { get; init; }
            public List<string> ContentTypes { get; init; }
            public long MaxFileSize { get; init; }
        }

        public string StorageUrl { get; init; }
        public StorageOptions AudioStorageOptions { get; init; }
        public StorageOptions ImageStorageOptions { get; init; }
    }
}