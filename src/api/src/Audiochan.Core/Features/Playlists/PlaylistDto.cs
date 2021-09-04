using System.Collections.Generic;
using System.Linq;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Users;
using Audiochan.Core.Features.Users.GetProfile;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Core.Features.Playlists
{
    public record PlaylistDto : IHasId<long>, IMapFrom<Playlist>
    {
        public long Id { get; init; }
        
        public string Title { get; init; } = string.Empty;
        
        public string Description { get; init; } = string.Empty;
        
        public string? Picture { get; init; }
        
        public List<string> Tags { get; init; } = new();
        
        public List<AudioDto> Audios { get; init; } = new();
        
        public UserDto User { get; init; } = null!;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Playlist, PlaylistDto>()
                .ForMember(dest => dest.Description, c =>
                    c.NullSubstitute(""))
                .ForMember(dest => dest.Picture, c =>
                {
                    c.MapFrom(src => src.Picture != null ? MediaLinkConstants.PlaylistPicture + src.Picture : null);
                })
                .ForMember(dest => dest.Tags, c =>
                    c.MapFrom(src => src.Tags.Select(t => t.Name).ToList()));
        }
    }
}