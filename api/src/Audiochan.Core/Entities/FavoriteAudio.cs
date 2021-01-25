using System;
using Audiochan.Core.Entities.Base;

namespace Audiochan.Core.Entities
{
    public class FavoriteAudio : BaseEntity
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public string AudioId { get; set; }
        public Audio Audio { get; set; }
    }
}
