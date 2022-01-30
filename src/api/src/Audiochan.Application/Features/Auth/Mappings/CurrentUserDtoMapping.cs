using Audiochan.Application.Features.Auth.Queries.GetCurrentUser;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Application.Features.Auth.Mappings;

public class CurrentUserDtoMapping : Profile
{
    public CurrentUserDtoMapping()
    {
        this.CreateStrictMap<User, CurrentUserDto>();
    }
}