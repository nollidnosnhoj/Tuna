namespace Audiochan.Core.Features.Auth.GetCurrentUser
{
    public record CurrentUserViewModel
    {
        public string Id { get; init; } = null!;
        public string UserName { get; init; } = null!;
        public string Email { get; init; } = null!;
    }
}