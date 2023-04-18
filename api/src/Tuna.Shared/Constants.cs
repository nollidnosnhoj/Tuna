namespace Tuna.Shared;

public static class ClaimNames
{
    public const string UserId = "userId";
    public const string HasProfile = "hasProfile";
}

public static class MediaConfigurationConstants
{
    public const int AUDIO_MAX_FILE_SIZE = 262144000;
    public const int IMAGE_MAX_FILE_SIZE = 2097152;
    public static string[] AUDIO_VALID_TYPES = { "audio/mp3", "audio/mpeg" };
    public static string[] IMAGE_VALID_TYPES = { "image/jpeg", "image/png", "image/gif" };
}

public static class AssetContainerConstants
{
    public const string AUDIO_STREAM = "audios";
}

public static class MediaLinkConstants
{
    public const string AUDIO_STREAM = "https://audiochan.s3.amazonaws.com/audios/";
}