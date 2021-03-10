using Audiochan.Core.Entities.Base;

namespace Audiochan.Core.Entities
{
    public class FollowedUser : BaseEntity
    {
        public string ObserverId { get; set; }
        public User Observer { get; set; }
        public string TargetId { get; set; }
        public User Target { get; set; }
    }
}