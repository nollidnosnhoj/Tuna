using Audiochan.Core.Commons.Extensions;
using Audiochan.Core.Features.Auth.Queries.GetCurrentUser;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Core.Features.Auth.Mappings;

public class CurrentUserDtoMapping : Profile
{
    public CurrentUserDtoMapping()
    {
        this.CreateStrictMap<User, CurrentUserDto>();
    }
}