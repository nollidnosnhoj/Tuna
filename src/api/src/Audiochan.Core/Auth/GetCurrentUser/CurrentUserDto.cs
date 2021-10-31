using Audiochan.Core.Common.Mappings;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Auth
{
    public record CurrentUserDto : IHasId<long>, IMapFrom<User>
    {
        public long Id { get; init; }
        public string UserName { get; init; } = null!;
        public string Email { get; init; } = null!;
    }
}