using System.Collections.Generic;

namespace Audiochan.Core.Common.Settings
{
    public record UploadSetting
    {
        public List<string> ContentTypes { get; init; } = new();
        public long FileSize { get; init; }
    }
}