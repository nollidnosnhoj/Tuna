using Audiochan.Core.Entities.Enums;

namespace Audiochan.Core.Entities.Abstractions
{
    public interface IHasVisibility
    {
        public string UserId { get; set; }
        public Visibility Visibility { get; set; }
    }
}