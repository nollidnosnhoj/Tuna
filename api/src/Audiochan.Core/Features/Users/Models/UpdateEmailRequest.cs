namespace Audiochan.Core.Features.Users.Models
{
    public record UpdateEmailRequest
    {
        public string? Email { get; } = string.Empty;
    }
}