namespace Audiochan.Core.Users
{
    public record UpdateEmailRequest
    {
        public string NewEmail { get; init; } = null!;
    }
}