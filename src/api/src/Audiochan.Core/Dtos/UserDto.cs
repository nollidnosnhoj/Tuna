using System.Text.Json.Serialization;
using Audiochan.Core.Converters.Json;
using Audiochan.Core.Interfaces;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Dtos
{
    public record UserDto : IMapFrom<User>
    {
        public long Id { get; init; }
        
        public string UserName { get; init; } = null!;
        
        [JsonConverter(typeof(UserPictureJsonConverter))]
        public string? Picture { get; init; }
    }
}