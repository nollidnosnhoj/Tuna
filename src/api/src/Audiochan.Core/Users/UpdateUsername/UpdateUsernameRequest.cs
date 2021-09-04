namespace Audiochan.Core.Users.UpdateUsername
{
    public record UpdateUsernameRequest
    {
        public string NewUsername { get; init; } = null!;
    }
}