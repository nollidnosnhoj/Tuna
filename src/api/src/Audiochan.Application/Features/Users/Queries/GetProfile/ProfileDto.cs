using System.Text.Json.Serialization;
using Audiochan.Application.Commons.Converters.Json;
using Audiochan.Application.Commons.Interfaces;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Entities;

namespace Audiochan.Application.Features.Users.Queries.GetProfile
{
    public record ProfileDto : IHasId<long>, IMapFrom<User>
    {
        public long Id { get; init; }
        
        public string UserName { get; init; } = null!;
        
        [JsonConverter(typeof(UserPictureJsonConverter))]
        public string? Picture { get; init; } = string.Empty;
        
        // public int AudioCount { get; init; }
        //
        // public int FollowerCount { get; init; }
        //
        // public int FollowingCount { get; init; }
    }
}