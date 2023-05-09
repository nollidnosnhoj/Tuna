namespace Tuna.Infrastructure.Images.CloudflareImages;

public class CloudflareSettings
{
    public class ImageSettings
    {
        public string ApiUrl { get; init; } = null!;
        public string ApiKey { get; init; } = null!;
    }
    
    public ImageSettings Images { get; init; } = null!;
}