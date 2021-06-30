using Audiochan.Core.Entities.Enums;

namespace Audiochan.Core.Entities.Abstractions
{
    public interface IVisible
    {
        public Visibility Visibility { get; set; }
        public string? PrivateKey { get; set; }
    }
}