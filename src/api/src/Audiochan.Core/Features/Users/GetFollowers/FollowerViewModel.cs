using System;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Mappings;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Core.Features.Users.GetFollowers
{
    public record FollowerViewModel : IMapFrom<FollowedUser>
    {
        public string UserName { get; init; } = null!;
        public string? Picture { get; init; }
        public DateTime FollowedDate { get; init; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<FollowedUser, FollowerViewModel>()
                .ForMember(dest => dest.UserName, c =>
                    c.MapFrom(src => src.Observer.UserName))
                .ForMember(dest => dest.Picture, c =>
                {
                    c.MapFrom(src => src.Observer.Picture != null ? MediaLinkConstants.UserPicture + src.Observer.Picture : null);
                });
        }
    }
}