using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Features.Users.Models;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Application.Features.Users.Mappings;

public class CurrentUserDtoMapping : Profile
{
    public CurrentUserDtoMapping()
    {
        this.CreateStrictMap<User, CurrentUserDto>();
    }
}