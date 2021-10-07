using System.Text.Json.Serialization;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Converters.Json;
using Audiochan.Core.Common.Mappings;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Core.Users
{
    public record UserDto : IMapFrom<User>
    {
        public long Id { get; init; }
        
        public string UserName { get; init; } = null!;
        
        [JsonConverter(typeof(UserPictureJsonConverter))]
        public string? Picture { get; init; }
    }
}