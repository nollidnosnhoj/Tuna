using System;

namespace Audiochan.Core.Features.Followers.GetFollowers
{
    public record FollowerViewModel
    {
        public string Username { get; init; } = null!;
        public string? Picture { get; init; }
        public DateTime FollowedDate { get; init; }
    }
}