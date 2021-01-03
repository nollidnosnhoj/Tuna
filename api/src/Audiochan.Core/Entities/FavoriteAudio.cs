using System;
using Audiochan.Core.Entities.Base;

namespace Audiochan.Core.Entities
{
    public class FavoriteAudio : BaseEntity
    {
        public long UserId { get; set; }
        public string AudioId { get; set; } = null!;
        public User User { get; set; } = null!;
        public Audio Audio { get; set; } = null!;
    }
}
