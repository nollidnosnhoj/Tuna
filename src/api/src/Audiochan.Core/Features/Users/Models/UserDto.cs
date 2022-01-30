using System.Text.Json.Serialization;
using Audiochan.Core.Commons.Converters.Json;
using Audiochan.Core.Commons.Interfaces;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Features.Users.Models
{
    public record UserDto : IMapFrom<User>
    {
        public long Id { get; init; }
        
        public string UserName { get; init; } = null!;
        
        [JsonConverter(typeof(UserPictureJsonConverter))]
        public string? Picture { get; init; }
    }
}