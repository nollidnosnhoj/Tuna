namespace Audiochan.Core.Features.Followers.GetFollowings
{
    public record FollowingViewModel
    {
        public string Username { get; init; }
        public string Picture { get; init; }
    }
}