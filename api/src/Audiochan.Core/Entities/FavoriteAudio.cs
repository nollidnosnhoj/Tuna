using System;

namespace Audiochan.Core.Entities
{
    public class FavoriteAudio
    {
        public Guid AudioId { get; set; }
        public Audio Audio { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
        
        public DateTime FavoriteDate { get; set; }
        public DateTime? UnfavoriteDate { get; set; }
    }
}