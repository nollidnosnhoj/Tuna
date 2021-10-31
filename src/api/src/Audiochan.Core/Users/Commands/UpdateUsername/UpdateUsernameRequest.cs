namespace Audiochan.Core.Users.Commands
{
    public record UpdateUsernameRequest
    {
        public string NewUsername { get; init; } = null!;
    }
}