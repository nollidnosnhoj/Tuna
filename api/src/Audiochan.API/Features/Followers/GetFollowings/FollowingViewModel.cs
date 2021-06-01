using System;

namespace Audiochan.API.Features.Followers.GetFollowings
{
    public record FollowingViewModel
    {
        public string Username { get; init; } = null!;
        public string? Picture { get; init; }
    }
}