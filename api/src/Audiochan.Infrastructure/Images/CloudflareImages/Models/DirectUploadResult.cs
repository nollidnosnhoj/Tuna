using Refit;

namespace Audiochan.Infrastructure.Images.CloudflareImages.Models;

public class DirectUploadResult
{
    [AliasAs("uploadURL")]
    public string UploadUrl { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
}