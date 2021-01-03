namespace Audiochan.Core.Common.Models
{
    /// <summary>
    /// This model contains information about the blob.
    /// </summary>
    public class BlobDto
    {
        public bool FoundBlob { get; set; }
        public string Container { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Url { get; set; } = null!;
        public long Size { get; set; }
    }
}
