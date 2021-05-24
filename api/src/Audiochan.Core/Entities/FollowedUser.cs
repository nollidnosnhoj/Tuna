using System;

namespace Audiochan.Core.Entities
{
    public class FollowedUser
    {
        public string ObserverId { get; set; } = null!;
        public User Observer { get; set; } = null!;
        public string TargetId { get; set; } = null!;
        public User Target { get; set; } = null!;
        
        public DateTime FollowedDate { get; set; }
        public DateTime? UnfollowedDate { get; set; }
    }
}