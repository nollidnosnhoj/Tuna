namespace Audiochan.Application.Features.Users.Commands.UpdateUsername
{
    public record UpdateUsernameRequest
    {
        public string NewUsername { get; init; } = null!;
    }
}