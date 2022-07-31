using Audiochan.Core.Auth.Queries;
using Audiochan.Core.Extensions;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.API.Features.Auth.Mappings;

public class CurrentUserDtoMapping : Profile
{
    public CurrentUserDtoMapping()
    {
        this.CreateStrictMap<User, CurrentUserDto>();
    }
}