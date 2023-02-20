namespace Audiochan.Core.Features.Upload.Dtos
{
    public record ImageUploadResponse(string? Url)
    {
        public static ImageUploadResponse ToUserImage(string? fileName)
        {
            var url = string.IsNullOrEmpty(fileName)
                ? null
                : MediaLinkConstants.USER_PICTURE + fileName;
            return new ImageUploadResponse(url);
        }

        public static ImageUploadResponse ToAudioImage(string? fileName)
        {
            var url = string.IsNullOrEmpty(fileName)
                ? null
                : MediaLinkConstants.AUDIO_PICTURE + fileName;
            return new ImageUploadResponse(url);
        }
    }
}