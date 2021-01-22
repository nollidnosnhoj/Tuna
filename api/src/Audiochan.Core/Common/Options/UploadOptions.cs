using System.Collections.Generic;

namespace Audiochan.Core.Common.Options
{
    public record UploadOptions
    {
        public List<string> ContentTypes { get; init; } = new();
        public long FileSize { get; init; }
    }
}