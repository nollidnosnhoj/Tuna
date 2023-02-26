using System;

namespace Audiochan.Core.Entities
{
    public class FollowedUser
    {
        public long ObserverId { get; set; }
        public User Observer { get; set; } = null!;
        public long TargetId { get; set; }
        public User Target { get; set; } = null!;
        
        public DateTime FollowedDate { get; set; }
        public DateTime? UnfollowedDate { get; set; }
    }
}