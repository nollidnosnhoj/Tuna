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
        public const string AudioPictureUrl = "https://audiochan.s3.amazonaws.com/images/audios/{0}";
        public const string PlaylistPictureUrl = "https://audiochan.s3.amazonaws.com/images/playlists/{0}";
        public const string AudioUrl = "https://audiochan.s3.amazonaws.com/audios/{0}";
        public const string UserPictureUrl = "https://audiochan.s3.amazonaws.com/images/users/{0}";
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