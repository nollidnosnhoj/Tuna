namespace Audiochan.Infrastructure.Storage.Options
{
    public class AmazonS3Options
    {
        public string PublicKey { get; set; }
        public string SecretKey { get; set; }
        public string Bucket { get; set; }
        public string Region { get; set; }
        public long ChunkThreshold { get; set; }
    }
}