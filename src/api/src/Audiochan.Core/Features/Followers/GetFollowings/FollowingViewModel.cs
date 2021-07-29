using System;

namespace Audiochan.Core.Features.Followers.GetFollowings
{
    public record FollowingViewModel
    {
        public string TargetUserName { get; init; } = null!;
        public string? TargetPicture { get; init; }
        public DateTime FollowedDate { get; init; }
    }
}