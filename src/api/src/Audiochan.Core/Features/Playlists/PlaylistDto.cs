using System.Collections.Generic;
using System.Text.Json.Serialization;
using Audiochan.Core.Common.Converters.Json;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Users;
using Audiochan.Domain.Abstractions;

namespace Audiochan.Core.Features.Playlists
{
    public record PlaylistDto : IHasId<long>
    {
        public long Id { get; init; }
        
        public string Title { get; init; } = string.Empty;
        
        public string Description { get; init; } = string.Empty;
        
        [JsonConverter(typeof(PlaylistPictureUrlJsonConverter))]
        public string? Picture { get; init; }
        
        public List<string> Tags { get; init; } = new();
        
        public List<AudioDto> Audios { get; init; } = new();
        
        public UserDto User { get; init; } = null!;
    }
}