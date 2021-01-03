namespace Audiochan.Infrastructure.Storage.Options
{
    public class AmazonS3Options
    {
        public string PublicKey { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
        public string Bucket { get; set; } = null!;
        public string? Region { get; set; }
        public long ChunkThreshold { get; set; }
    }
}