namespace Audiochan.Core.Users.UpdateEmail
{
    public record UpdateEmailRequest
    {
        public string NewEmail { get; init; } = null!;
    }
}