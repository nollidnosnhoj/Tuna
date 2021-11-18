using System.Text.Json.Serialization;
using Audiochan.Core.Common.Converters.Json;
using Audiochan.Core.Common.Mappings;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Users
{
    public record ArtistDto : IMapFrom<Artist>
    {
        public long Id { get; init; }
        public string UserName { get; init; } = null!;
        public string DisplayName { get; init; } = null!;
        
        [JsonConverter(typeof(UserPictureJsonConverter))]
        public string? Picture { get; init; }
    }
}