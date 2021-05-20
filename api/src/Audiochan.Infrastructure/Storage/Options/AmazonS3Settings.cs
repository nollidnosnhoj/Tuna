namespace Audiochan.Infrastructure.Storage.Options
{
    public record AmazonS3Settings
    {
        public string PublicKey { get; init; } = null!;
        public string SecretKey { get; init; } = null!;
        public string Region { get; init; } = null!;
    }
}