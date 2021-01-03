namespace Audiochan.Core.Entities
{
    public class AudioTag
    {
        public string AudioId { get; set; } = null!;
        public Audio Audio { get; set; } = null!;
        public string TagId { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }
}
