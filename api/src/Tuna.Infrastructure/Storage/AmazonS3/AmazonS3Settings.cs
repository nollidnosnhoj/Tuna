namespace Tuna.Infrastructure.Storage.AmazonS3;

public record AmazonS3Settings
{
    public string PublicKey { get; init; } = null!;
    public string SecretKey { get; init; } = null!;
    public string Region { get; init; } = null!;
}