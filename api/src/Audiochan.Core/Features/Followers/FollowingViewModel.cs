using System;

namespace Audiochan.Core.Features.Followers
{
    public record FollowingViewModel
    {
        public string Username { get; init; } = null!;
        public string? Picture { get; init; }
        public DateTime FollowedDate { get; init; }
    }
}