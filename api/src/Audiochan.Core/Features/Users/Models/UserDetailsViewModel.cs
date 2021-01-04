using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Features.Users.Models
{
    public record UserDetailsViewModel
    {
        public long Id { get; set; }
        public string Username { get; set; } = null!;
        public string AboutMe { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public int AudioCount { get; set; }
        public bool IsFollowing { get; set; }
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }

        public static UserDetailsViewModel From(User user, long currentUserId)
        {
            return MapProjections.UserDetails(currentUserId).Compile().Invoke(user);
        }
    }
}