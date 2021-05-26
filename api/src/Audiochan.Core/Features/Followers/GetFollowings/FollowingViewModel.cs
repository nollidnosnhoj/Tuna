using System;

namespace Audiochan.Core.Features.Followers.GetFollowings
{
    public record FollowingViewModel
    {
        public string Username { get; init; } = null!;
        public string? Picture { get; init; }
        public DateTime FollowedDate { get; init; }
    }
}