namespace Audiochan.Core.Features.Auth.Models
{
    public record LoginRequest
    {
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}