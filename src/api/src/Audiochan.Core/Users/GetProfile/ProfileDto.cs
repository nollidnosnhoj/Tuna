using System.Text.Json.Serialization;
using Audiochan.Core.Common.Converters.Json;
using Audiochan.Core.Common.Mappings;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Users
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