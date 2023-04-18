using Refit;

namespace Tuna.Infrastructure.Images.CloudflareImages.Models;

public class DirectUploadRequest
{
    [AliasAs("RequireSignedURLs")] public bool RequireSignedUrls { get; set; }

    public string? Metadata { get; set; }
    public string? Id { get; set; }
}