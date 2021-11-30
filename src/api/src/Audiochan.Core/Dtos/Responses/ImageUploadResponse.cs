namespace Audiochan.Core.Dtos.Responses
{
    public record ImageUploadResponse
    {
        public string Url { get; init; } = null!;
    }
}