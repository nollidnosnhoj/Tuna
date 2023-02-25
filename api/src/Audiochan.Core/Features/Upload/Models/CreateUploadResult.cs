namespace Audiochan.Core.Features.Upload.Models;

public record CreateUploadResult
{
    public string UploadId { get; init; } = null!;
    public string UploadUrl { get; init; } = null!;
}