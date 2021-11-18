using System.Text.Json.Serialization;
using Audiochan.Core.Common.Converters.Json;
using Audiochan.Core.Common.Mappings;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Artists.Queries
{
    public record ArtistProfileDto : IHasId<long>, IMapFrom<Artist>
    {
        public long Id { get; init; }
        public string UserName { get; init; } = null!;
        public string DisplayName { get; set; } = null!;
        [JsonConverter(typeof(UserPictureJsonConverter))]
        public string? Picture { get; init; } = string.Empty;
    }
}