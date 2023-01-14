namespace Audiochan.Core.Features.Upload.Commands.CreateUpload
{
    public record GenerateUploadLinkResponse
    {
        public string UploadId { get; init; } = null!;
        public string UploadUrl { get; init; } = null!;
    }
}