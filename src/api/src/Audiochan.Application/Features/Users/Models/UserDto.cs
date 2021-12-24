using System.Text.Json.Serialization;
using Audiochan.Application.Commons.Converters.Json;
using Audiochan.Application.Commons.Interfaces;
using Audiochan.Domain.Entities;

namespace Audiochan.Application.Features.Users.Models
{
    public record UserDto : IMapFrom<User>
    {
        public long Id { get; init; }
        
        public string UserName { get; init; } = null!;
        
        [JsonConverter(typeof(UserPictureJsonConverter))]
        public string? Picture { get; init; }
    }
}