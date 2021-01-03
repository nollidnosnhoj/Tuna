using System;
using Audiochan.Core.Entities.Base;

namespace Audiochan.Core.Entities
{
    public class FollowedUser : BaseEntity
    {
        public long ObserverId { get; set; }
        public User Observer { get; set; } = null!;
        public long TargetId { get; set; }
        public User Target { get; set; } = null!;
    }
}
