namespace Audiochan.Core.Features.Followers
{
    public record FollowerViewModel
    {
        public string Username { get; init; } = null!;
        public string? Picture { get; init; }
    }
}