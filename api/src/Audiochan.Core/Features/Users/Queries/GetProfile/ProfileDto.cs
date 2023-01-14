using Audiochan.Domain.Abstractions;

namespace Audiochan.Core.Features.Users.Queries.GetProfile
{
    public record ProfileDto : IHasId<long>
    {
        public long Id { get; init; }
        public string UserName { get; init; } = null!;
        public string? Picture { get; init; } = string.Empty;
    }
}