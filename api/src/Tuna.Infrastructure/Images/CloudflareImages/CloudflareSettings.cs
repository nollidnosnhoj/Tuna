namespace Tuna.Infrastructure.Images.CloudflareImages;

public class CloudflareSettings
{
    public ImageSettings Images { get; init; } = null!;

    public class ImageSettings
    {
        public string ApiUrl { get; init; } = null!;
        public string ApiKey { get; init; } = null!;
    }
}