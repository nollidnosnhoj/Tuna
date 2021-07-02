using System;

namespace Audiochan.Core.Common.Constants
{
    public static class CacheKeys
    {
        public static class Audio
        {
            public static string GetAudio(Guid audioId) => $"audio_id_{audioId}";
        }
    }
}