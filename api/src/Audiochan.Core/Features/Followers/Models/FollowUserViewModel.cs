namespace Audiochan.Core.Features.Followers.Models
{
    public record FollowUserViewModel
    {
        public string Id { get; init; }
        public string Username { get; init; }
        public string PictureUrl { get; init; } = string.Empty;
        public bool IsFollowing { get; init; }
    }
}