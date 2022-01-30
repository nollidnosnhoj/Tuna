using Audiochan.Application.Commons.Interfaces;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Entities;

namespace Audiochan.Application.Features.Users.Models
{
    public record CurrentUserDto : IHasId<long>, IMapFrom<User>
    {
        public long Id { get; init; }
        public string UserName { get; init; } = null!;
        public string Email { get; init; } = null!;
    }
}