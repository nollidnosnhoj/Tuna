namespace Audiochan.Core.Common
{
    public static class AssetContainerConstants
    {
        public const string AudioPictures = "images/audios";
        public const string PlaylistPictures = "images/playlists";
        public const string UserPictures = "images/users";
    }
    
    public static class CacheKeys
    {
        public static class Audio
        {
            public static string GetAudio(long audioId) => $"audio_id_{audioId}";
        }
    }
    
    public static class MediaLinkInvariants
    {
        public const string AudioPicture = "https://audiochan.s3.amazonaws.com/images/audios/";
        public const string PlaylistPicture = "https://audiochan.s3.amazonaws.com/images/playlists/";
        public const string AudioStream = "https://audiochan.s3.amazonaws.com/audios/";
        public const string UserPicture = "https://audiochan.s3.amazonaws.com/images/users/";
    }
    
    public static class ValidationErrorCodes
    {
        public static class Password
        {
            public const string Digits = "requireDigits";
            public const string Lowercase = "requireLowercase";
            public const string Uppercase = "requireUppercase";
            public const string NonAlphanumeric = "requireNonAlphanumeric";
            public const string Length = "requireLength";
        }

        public static class Username
        {
            public const string Characters = "requireCharacters";
            public const string Format = "invalidFormat";
        }
    }
}