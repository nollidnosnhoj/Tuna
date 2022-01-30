namespace Audiochan.Core.Features.Users.Commands.UpdateEmail
{
    public record UpdateEmailRequest
    {
        public string NewEmail { get; init; } = null!;
    }
}