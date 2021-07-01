using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Auth.GetCurrentUser;
using Audiochan.Core.Features.Users.GetProfile;
using AutoMapper;

namespace Audiochan.Core.Features.Users
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, MetaAuthorDto>()
                .ForMember(dest => dest.Picture, opts =>
                    opts.MapFrom(src => src.PictureBlobName != null
                        ? string.Format(MediaLinkInvariants.UserPictureUrl, src.PictureBlobName)
                        : null));

            CreateMap<User, CurrentUserViewModel>();

            CreateMap<User, ProfileViewModel>()
                .ForMember(dest => dest.Picture, opts =>
                    opts.MapFrom(src => src.PictureBlobName != null
                        ? string.Format(MediaLinkInvariants.UserPictureUrl, src.PictureBlobName)
                        : null));
        }
    }
}