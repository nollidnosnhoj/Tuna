namespace Audiochan.Core.Features.Users.Models
{
    public record UpdateUserDetailsRequest
    {
        public string? About { get; init; } = string.Empty;
        public string? Website { get; init; } = string.Empty;
    }
}