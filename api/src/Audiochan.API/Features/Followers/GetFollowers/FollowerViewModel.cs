using System;

namespace Audiochan.API.Features.Followers.GetFollowers
{
    public record FollowerViewModel
    {
        public string Username { get; init; } = null!;
        public string? Picture { get; init; }
    }
}