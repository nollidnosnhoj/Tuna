using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Features.Followers.Models
{
    public record FollowUserViewModel
    {
        public long Id { get; init; }
        public string Username { get; init; } = null!;
        public string AvatarUrl { get; init; } = string.Empty;
        public bool IsFollowing { get; init; }

        public static FollowUserViewModel From(FollowedUser followedUser, long currentUserId)
        {
            return MapProjections.FollowUser(currentUserId).Compile().Invoke(followedUser);
        }
    }
}