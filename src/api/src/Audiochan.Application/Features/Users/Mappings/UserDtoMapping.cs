using Audiochan.Application.Features.Users.Models;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Application.Features.Users.Mappings;

public class UserDtoMapping : Profile
{
    public UserDtoMapping()
    {
        this.CreateStrictMap<User, UserDto>();
    }
}

public class FollowedUserDtoMapping : Profile
{
    public FollowedUserDtoMapping()
    {
        this.CreateStrictMap<FollowedUser, FollowedUserDto>();
    }
}