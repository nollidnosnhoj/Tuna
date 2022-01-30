using Audiochan.Application.Features.Users.Queries.GetProfile;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Features.Users.Models;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Application.Features.Users.Mappings;

public class ProfileDtoMapping : Profile
{
    public ProfileDtoMapping()
    {
        this.CreateStrictMap<User, ProfileDto>();
    }
}