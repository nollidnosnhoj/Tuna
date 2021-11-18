using System;

namespace Audiochan.Domain.Entities
{
    public class FollowedArtist
    {
        public long ObserverId { get; set; }
        public User Observer { get; set; } = null!;
        public long TargetId { get; set; }
        public Artist Target { get; set; } = null!;
        
        public DateTime FollowedDate { get; set; }
        public DateTime? UnfollowedDate { get; set; }
    }
}