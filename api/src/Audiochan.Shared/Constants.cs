namespace Audiochan.Shared;

public static class ClaimNames
{
    public const string UserId = "userId";
    public const string HasProfile = "hasProfile";
}

public static class MediaConfigurationConstants
{
    public const int AUDIO_MAX_FILE_SIZE = 262144000;
    public static string[] AUDIO_VALID_TYPES = { "audio/mp3", "audio/mpeg" };
    public const int IMAGE_MAX_FILE_SIZE = 2097152;
    public static string[] IMAGE_VALID_TYPES = { "image/jpeg", "image/png", "image/gif" };
}
    
public static class AssetContainerConstants
{
    public const string AUDIO_STREAM = "audios";
    public const string AUDIO_PICTURES = "images/audios";
    public const string PLAYLIST_PICTURES = "images/playlists";
    public const string USER_PICTURES = "images/users";
}

public static class MediaLinkConstants
{
    public const string AUDIO_PICTURE = "https://audiochan.s3.amazonaws.com/images/audios/";
    public const string PLAYLIST_PICTURE = "https://audiochan.s3.amazonaws.com/images/playlists/";
    public const string AUDIO_STREAM = "https://audiochan.s3.amazonaws.com/audios/";
    public const string USER_PICTURE = "https://audiochan.s3.amazonaws.com/images/users/";
}