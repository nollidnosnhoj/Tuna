namespace Audiochan.Core.Features.Followers.GetFollowers
{
    public record FollowerViewModel
    {
        public string Username { get; init; }
        public string Picture { get; init; }
    }
}