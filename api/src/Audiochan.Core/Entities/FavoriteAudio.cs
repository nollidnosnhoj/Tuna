using System;
using Audiochan.Core.Entities.Abstractions;

namespace Audiochan.Core.Entities
{
    public class FavoriteAudio : IFavorited
    {
        public long AudioId { get; set; }
        public Audio Audio { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
        public DateTime FavoriteDate { get; set; }
        public DateTime? UnfavoriteDate { get; set; }
    }
}