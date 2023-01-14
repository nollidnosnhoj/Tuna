namespace Audiochan.Common.Dtos
{
    public record ImageUploadRequest
    {
        public string Data { get; init; } = string.Empty;
    }
}