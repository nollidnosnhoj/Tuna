namespace Audiochan.Core.Users.Commands
{
    public record UpdateEmailRequest
    {
        public string NewEmail { get; init; } = null!;
    }
}