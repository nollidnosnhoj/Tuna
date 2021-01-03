using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Features.Followers.Models
{
    public record FollowUserViewModel
    {
        public long Id { get; set; }
        public string Username { get; set; } = null!;
        public string AvatarUrl { get; set; } = string.Empty;
        public bool IsFollowing { get; set; }

        public static FollowUserViewModel From(FollowedUser followedUser, long currentUserId)
        {
            return MapProjections.FollowUser(currentUserId).Compile().Invoke(followedUser);
        }
    }
}