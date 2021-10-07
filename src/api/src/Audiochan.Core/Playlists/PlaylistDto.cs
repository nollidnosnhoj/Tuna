using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Audiochan.Core.Audios;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Converters.Json;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Users;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Core.Playlists
{
    public record PlaylistDto : IHasId<long>, IMapFrom<Playlist>
    {
        public long Id { get; init; }
        
        public string Title { get; init; } = string.Empty;
        
        public string Description { get; init; } = string.Empty;
        
        [JsonConverter(typeof(PlaylistPictureJsonConverter))]
        public string? Picture { get; init; }
        
        public List<string> Tags { get; init; } = new();
        
        public List<AudioDto> Audios { get; init; } = new();
        
        public UserDto User { get; init; } = null!;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Playlist, PlaylistDto>()
                .ForMember(dest => dest.Description, c =>
                    c.NullSubstitute(""));
        }
    }
}