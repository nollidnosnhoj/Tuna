using System;

namespace Audiochan.Core.Features.Users.Queries.GetFollowings
{
    public record FollowingViewModel
    {
        public string UserName { get; init; } = null!;
        
        public string? Picture { get; init; }
        
        public DateTime FollowedDate { get; init; }
    }
}