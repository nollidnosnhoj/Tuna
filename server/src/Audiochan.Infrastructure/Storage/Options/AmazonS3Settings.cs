namespace Audiochan.Infrastructure.Storage.Options
{
    public record AmazonS3Settings
    {
        public string PublicKey { get; init; }
        public string SecretKey { get; init; }
        public string Region { get; init; }
    }
}