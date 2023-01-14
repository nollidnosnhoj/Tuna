using System;

namespace Audiochan.Core.Features.Users.Queries.GetFollowers
{
    public record FollowerViewModel 
    {
        public string UserName { get; init; } = null!;
        public string? Picture { get; init; }
        public DateTime FollowedDate { get; init; }
    }
}