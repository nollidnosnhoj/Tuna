namespace Audiochan.Core.Features.Auth.GetCurrentUser
{
    public record CurrentUserViewModel
    {
        public long Id { get; init; }
        public string Username { get; init; } = null!;
        public string Email { get; init; } = null!;
    }
}