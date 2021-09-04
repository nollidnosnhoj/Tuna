using Audiochan.Core.Common;
using Audiochan.Core.Common.Mappings;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Core.Users
{
    public record UserDto : IMapFrom<User>
    {
        public long Id { get; init; }
        public string UserName { get; init; } = null!;
        public string? Picture { get; init; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserDto>()
                .ForMember(dest => dest.Picture, c =>
                {
                    c.MapFrom(src => src.Picture != null ? MediaLinkConstants.UserPicture + src.Picture : null);
                });
        }
    }
}