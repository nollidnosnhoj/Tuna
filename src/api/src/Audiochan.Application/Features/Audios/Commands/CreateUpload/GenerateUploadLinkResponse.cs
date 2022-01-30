namespace Audiochan.Application.Features.Audios.Commands.CreateUpload
{
    public record GenerateUploadLinkResponse
    {
        public string UploadId { get; init; } = null!;
        public string UploadUrl { get; init; } = null!;
    }
}