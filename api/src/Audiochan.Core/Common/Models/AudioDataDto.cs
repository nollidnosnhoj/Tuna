namespace Audiochan.Core.Common.Models
{
    /// <summary>
    /// Contains metadata from an audio file.
    /// </summary>
    public class AudioDataDto
    {
        public string? Title { get; set; }
        public int Duration { get; set; }
        public int BitRate { get; set; }
        public double SampleRate { get; set; }
    }
}