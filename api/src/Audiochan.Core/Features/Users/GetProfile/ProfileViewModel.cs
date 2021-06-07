namespace Audiochan.Core.Features.Users.GetProfile
{
    public record ProfileViewModel
    {
        public string Id { get; init; } = null!;
        public string Username { get; init; } = null!;
        public string About { get; init; } = string.Empty;
        public string Website { get; init; } = string.Empty;
        public string? Picture { get; init; } = string.Empty;
        public int AudioCount { get; init; }
        public bool? IsFollowing { get; init; }
        public int FollowerCount { get; init; }
        public int FollowingCount { get; init; }
    }
}