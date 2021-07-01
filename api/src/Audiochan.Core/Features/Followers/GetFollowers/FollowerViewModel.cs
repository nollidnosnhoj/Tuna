using System;

namespace Audiochan.Core.Features.Followers.GetFollowers
{
    public record FollowerViewModel
    {
        public string ObserverUserName { get; init; } = null!;
        public string? ObserverPicture { get; init; }
        public DateTime FollowedDate { get; init; }
    }
}