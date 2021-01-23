using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Features.Users.Models
{
    public record UserDetailsViewModel
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string AboutMe { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public int AudioCount { get; set; }
        public bool IsFollowing { get; set; }
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }

        public static UserDetailsViewModel From(User user, string currentUserId)
        {
            return MapProjections.UserDetails(currentUserId).Compile().Invoke(user);
        }
    }
}