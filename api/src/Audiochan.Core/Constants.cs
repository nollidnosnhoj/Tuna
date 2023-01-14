namespace Audiochan.Core
{
    public static class AssetContainerConstants
    {
        public const string AUDIO_PICTURES = "images/audios";
        public const string PLAYLIST_PICTURES = "images/playlists";
        public const string USER_PICTURES = "images/users";
    }
    
    public static class CacheKeys
    {
        public static class Audio
        {
            public static string GetAudio(long audioId) => $"audio_id_{audioId}";
        }
    }
    
    public static class MediaLinkConstants
    {
        public const string AUDIO_PICTURE = "https://audiochan.s3.amazonaws.com/images/audios/";
        public const string PLAYLIST_PICTURE = "https://audiochan.s3.amazonaws.com/images/playlists/";
        public const string AUDIO_STREAM = "https://audiochan.s3.amazonaws.com/audios/";
        public const string USER_PICTURE = "https://audiochan.s3.amazonaws.com/images/users/";
    }
}