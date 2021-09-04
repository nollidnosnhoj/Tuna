using Audiochan.Core.Common;
using Audiochan.Core.Common.Mappings;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Core.Users.GetProfile
{
    public record ProfileDto : IHasId<long>, IMapFrom<User>
    {
        public long Id { get; init; }
        
        public string UserName { get; init; } = null!;
        public string? Picture { get; init; } = string.Empty;
        
        // public int AudioCount { get; init; }
        //
        // public int FollowerCount { get; init; }
        //
        // public int FollowingCount { get; init; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, ProfileDto>()
                .ForMember(dest => dest.Picture, c =>
                {
                    c.MapFrom(src => src.Picture != null ? MediaLinkConstants.UserPicture + src.Picture : null);
                });
        }
    }
}