using Audiochan.Core.Dtos;
using Audiochan.Core.Extensions;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.API.Features.Users.Mappings;

public class UserDtoMapping : Profile
{
    public UserDtoMapping()
    {
        this.CreateStrictMap<User, UserDto>();
    }
}