namespace Audiochan.Core.Features.Upload.Models;

public record ImageUploadResult(string? Url)
{
    public static ImageUploadResult ToUserImage(string? fileName)
    {
        var url = string.IsNullOrEmpty(fileName)
            ? null
            : MediaLinkConstants.USER_PICTURE + fileName;
        return new ImageUploadResult(url);
    }

    public static ImageUploadResult ToAudioImage(string? fileName)
    {
        var url = string.IsNullOrEmpty(fileName)
            ? null
            : MediaLinkConstants.AUDIO_PICTURE + fileName;
        return new ImageUploadResult(url);
    }
}