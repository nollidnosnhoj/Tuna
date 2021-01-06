namespace Audiochan.Core.Features.Auth.Models
{
    public record CreateUserRequest
    {
        public string? Username { get; init; } = string.Empty;
        public string? Email { get; init; } = string.Empty;
        public string? Password { get; init; } = string.Empty;
    }
}