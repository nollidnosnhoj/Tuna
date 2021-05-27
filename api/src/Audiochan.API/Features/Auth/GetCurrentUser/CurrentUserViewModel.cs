namespace Audiochan.API.Features.Auth.GetCurrentUser
{
    public record CurrentUserViewModel
    {
        public string Id { get; init; } = null!;
        public string Username { get; init; } = null!;
        public string Email { get; init; } = null!;
    }
}