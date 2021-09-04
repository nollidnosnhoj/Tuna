using Audiochan.Core.Common.Mappings;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Core.Features.Auth.GetCurrentUser
{
    public record CurrentUserDto : IHasId<long>, IMapFrom<User>
    {
        public long Id { get; init; }
        public string UserName { get; init; } = null!;
        public string Email { get; init; } = null!;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, CurrentUserDto>();
        }
    }
}