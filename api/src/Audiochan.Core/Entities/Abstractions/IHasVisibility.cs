using Audiochan.Core.Entities.Enums;

namespace Audiochan.Core.Entities.Abstractions
{
    public interface IHasVisibility
    {
        public Visibility Visibility { get; set; }
        public string? Secret { get; set; }
    }
}