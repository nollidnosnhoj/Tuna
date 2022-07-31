using Audiochan.Core.Extensions;
using Audiochan.Core.Users.Queries;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.API.Features.Users.Mappings;

public class ProfileDtoMapping : Profile
{
    public ProfileDtoMapping()
    {
        this.CreateStrictMap<User, ProfileDto>();
    }
}