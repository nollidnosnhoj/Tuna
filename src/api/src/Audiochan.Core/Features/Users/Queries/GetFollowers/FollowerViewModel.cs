using System;
using Audiochan.Core.Commons.Interfaces;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Core.Features.Users.Queries.GetFollowers
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
                    c.MapFrom(src => src.Observer.Picture != null ? MediaLinkConstants.USER_PICTURE + src.Observer.Picture : null);
                });
        }
    }
}