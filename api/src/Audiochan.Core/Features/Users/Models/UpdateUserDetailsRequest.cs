namespace Audiochan.Core.Features.Users.Models
{
    public record UpdateUserDetailsRequest
    {
        public string? About { get; }
        public string? Website { get; }
    }
}