namespace Audiochan.Core.Features.Auth
{
    public record CurrentUserViewModel
    {
        public string Id { get; init; }
        public string Username { get; init; }
        public string Email { get; init; }
    }
}