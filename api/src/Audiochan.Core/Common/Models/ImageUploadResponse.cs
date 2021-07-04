namespace Audiochan.Core.Common.Models
{
    public record ImageUploadResponse
    {
        public string Url { get; init; } = null!;
    }
}