using Audiochan.Domain.Abstractions;

namespace Audiochan.Core.Features.Auth.Queries.GetCurrentUser
{
    public record CurrentUserDto : IHasId<long>
    {
        public long Id { get; init; }
        public string UserName { get; init; } = null!;
        public string Email { get; init; } = null!;
    }
}