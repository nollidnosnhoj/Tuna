namespace Audiochan.Core.Features.Users
{
    public record UserDto
    {
        public long Id { get; init; }
        public string Username { get; init; } = null!;
        public string? Picture { get; init; }
    }
}