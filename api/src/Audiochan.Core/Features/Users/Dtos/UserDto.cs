namespace Audiochan.Core.Features.Users.Dtos
{
    public record UserDto
    {
        public long Id { get; init; }
        public string UserName { get; init; } = null!;
        public string? Picture { get; init; }
    }
}