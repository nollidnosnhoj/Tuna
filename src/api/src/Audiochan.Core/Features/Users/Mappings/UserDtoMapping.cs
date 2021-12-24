using Audiochan.Core.Commons.Extensions;
using Audiochan.Core.Features.Users.Models;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Core.Features.Users.Mappings;

public class UserDtoMapping : Profile
{
    public UserDtoMapping()
    {
        this.CreateStrictMap<User, UserDto>();
    }
}