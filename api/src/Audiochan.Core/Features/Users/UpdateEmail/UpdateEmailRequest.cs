namespace Audiochan.Core.Features.Users.UpdateEmail
{
    public record UpdateEmailRequest
    {
        public string NewEmail { get; init; } = null!;
    }
}