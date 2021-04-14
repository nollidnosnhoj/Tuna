namespace Audiochan.Core.Models.ViewModels
{
    public record UserViewModel
    {
        public string Id { get; init; }
        public string Username { get; init; }
        public string About { get; init; } = string.Empty;
        public string Website { get; init; } = string.Empty;
        public string Picture { get; init; } = string.Empty;
        public int AudioCount { get; init; }
        public bool? IsFollowing { get; init; }
        public int FollowerCount { get; init; }
        public int FollowingCount { get; init; }
    }
}