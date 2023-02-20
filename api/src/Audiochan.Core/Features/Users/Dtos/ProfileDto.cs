using Audiochan.Domain.Abstractions;

namespace Audiochan.Core.Features.Users.Dtos
{
    public record ProfileDto
    {
        public long Id { get; init; }
        public string UserName { get; init; } = null!;
        public string? Picture { get; init; } = string.Empty;
    }
}