namespace Audiochan.Core.Features.Users.UpdateUsername
{
    public record UpdateUsernameRequest
    {
        public string NewUsername { get; init; } = null!;
    }
}