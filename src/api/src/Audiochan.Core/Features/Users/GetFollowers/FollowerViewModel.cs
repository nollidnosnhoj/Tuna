using System;
using System.Text.Json.Serialization;
using Audiochan.Core.Common.Converters.Json;

namespace Audiochan.Core.Features.Users.GetFollowers
{
    public record FollowerViewModel
    {
        public string ObserverUserName { get; init; } = null!;
        
        [JsonConverter(typeof(UserPictureUrlJsonConverter))]
        public string? ObserverPicture { get; init; }
        
        public DateTime FollowedDate { get; init; }
    }
}