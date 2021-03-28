namespace Audiochan.Core.Common.Models.Responses
{
    public record GetPresignedUrlResponse
    {
        public string Url { get; init; }
    }
}