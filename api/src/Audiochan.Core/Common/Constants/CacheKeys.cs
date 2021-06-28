namespace Audiochan.Core.Common.Constants
{
    public static class CacheKeys
    {
        public static class Audio
        {
            public static string GetAudio(long audioId) => $"audio_id_{audioId}";
        }
    }
}