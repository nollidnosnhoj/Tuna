namespace Audiochan.Core.Users
{
    public record UpdateUsernameRequest
    {
        public string NewUsername { get; init; } = null!;
    }
}