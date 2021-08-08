using System;
using Audiochan.Core.Entities.Abstractions;

namespace Audiochan.Core.Entities
{
    public class FavoriteAudio
    {
        public Guid AudioId { get; set; }
        public Audio Audio { get; set; } = null!;
        public long UserId { get; set; }
        public User User { get; set; } = null!;
    }
}