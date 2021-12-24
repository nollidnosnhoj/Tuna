using Audiochan.Core.Commons.Extensions;
using Audiochan.Core.Features.Users.Queries.GetProfile;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Core.Features.Users.Mappings;

public class ProfileDtoMapping : Profile
{
    public ProfileDtoMapping()
    {
        this.CreateStrictMap<User, ProfileDto>();
    }
}