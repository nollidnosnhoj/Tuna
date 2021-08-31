using Audiochan.Core.Common.Interfaces;

namespace Audiochan.Core.Features.Users.GetProfile
{
    public record ProfileDto : IResourceDto<long>
    {
        public long Id { get; init; }
        public string Username { get; init; } = null!;
        public string About { get; init; } = string.Empty;
        public string Website { get; init; } = string.Empty;
        public string? Picture { get; init; } = string.Empty;
        public int AudioCount { get; init; }
        public int FollowerCount { get; init; }
        public int FollowingCount { get; init; }
    }
}