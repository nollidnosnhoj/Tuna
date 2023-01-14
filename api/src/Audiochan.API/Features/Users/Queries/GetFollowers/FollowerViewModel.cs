using System;
using Audiochan.Core.Interfaces;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Users.Queries
{
    public record FollowerViewModel : IMapFrom<FollowedUser>
    {
        public string UserName { get; init; } = null!;
        public string? Picture { get; init; }
        public DateTime FollowedDate { get; init; }
    }
}