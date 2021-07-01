using Audiochan.Core.Common.Constants;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Followers.GetFollowers;
using Audiochan.Core.Features.Followers.GetFollowings;
using AutoMapper;

namespace Audiochan.Core.Features.Followers
{
    public class FollowedUserMappingProfile : Profile
    {
        public FollowedUserMappingProfile()
        {
            CreateMap<FollowedUser, FollowerViewModel>()
                .ForMember(dest => dest.ObserverPicture, opts =>
                    opts.MapFrom(src => src.Observer.PictureBlobName != null
                        ? string.Format(MediaLinkInvariants.UserPictureUrl, src.Observer.PictureBlobName)
                        : null));
            
            CreateMap<FollowedUser, FollowingViewModel>()
                .ForMember(dest => dest.TargetPicture, opts =>
                    opts.MapFrom(src => src.Target.PictureBlobName != null
                        ? string.Format(MediaLinkInvariants.UserPictureUrl, src.Target.PictureBlobName)
                        : null));
        }
    }
}