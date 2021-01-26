namespace Audiochan.Core.Features.Users.Models
{
    public record UserDetailsViewModel
    {
        public string Id { get; init; }
        public string Username { get; init; }
        public string AboutMe { get; init; } = string.Empty;
        public string Website { get; init; } = string.Empty;
        public string PictureUrl { get; init; } = string.Empty;
        public int AudioCount { get; init; }
        public bool IsFollowing { get; init; }
        public int FollowerCount { get; init; }
        public int FollowingCount { get; init; }
    }
}