using Audiochan.Core.Commons.Interfaces;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Features.Auth.Queries.GetCurrentUser
{
    public record CurrentUserDto : IHasId<long>, IMapFrom<User>
    {
        public long Id { get; init; }
        public string UserName { get; init; } = null!;
        public string Email { get; init; } = null!;
    }
}