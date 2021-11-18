using System;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Mappings;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Core.Users.Queries
{
    public record FollowingViewModel : IMapFrom<FollowedArtist>
    {
        public string UserName { get; init; } = null!;
        
        public DateTime FollowedDate { get; init; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<FollowedArtist, FollowingViewModel>()
                .ForMember(dest => dest.UserName, c =>
                    c.MapFrom(src => src.Target.UserName));
        }
    }
}