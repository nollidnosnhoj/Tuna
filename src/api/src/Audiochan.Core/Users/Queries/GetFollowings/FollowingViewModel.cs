using System;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Mappings;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Core.Users.Queries
{
    public record FollowingViewModel : IMapFrom<FollowedUser>
    {
        public string UserName { get; init; } = null!;
        
        public string? Picture { get; init; }
        
        public DateTime FollowedDate { get; init; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<FollowedUser, FollowingViewModel>()
                .ForMember(dest => dest.UserName, c =>
                    c.MapFrom(src => src.Target.UserName))
                .ForMember(dest => dest.Picture, c =>
                {
                    c.MapFrom(src => src.Target.Picture != null ? MediaLinkConstants.USER_PICTURE + src.Target.Picture : null);
                });
        }
    }
}