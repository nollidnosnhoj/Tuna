namespace Audiochan.Core.Features.Users.GetCurrentUser
{
    public record CurrentUserViewModel
    {
        public string Id { get; init; }
        public string Username { get; init; }
        public string Email { get; init; }
    }
}