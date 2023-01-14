namespace Audiochan.Common.Dtos
{
    public record ImageUploadResponse
    {
        public string Url { get; init; } = null!;
    }
}