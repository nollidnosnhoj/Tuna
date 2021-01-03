namespace Audiochan.Core.Features.Tags.Models
{
    /// <summary>
    /// Used to return data about a tag
    /// </summary>
    public class PopularTagViewModel
    {
        public string Tag { get; set; } = null!;
        public int Count { get; set; }
    }
}