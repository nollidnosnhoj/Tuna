using System;
using System.Text.Json.Serialization;
using Audiochan.Core.Common.Converters.Json;

namespace Audiochan.Core.Features.Users.GetFollowings
{
    public record FollowingViewModel
    {
        public string TargetUserName { get; init; } = null!;
        
        [JsonConverter(typeof(UserPictureUrlJsonConverter))]
        public string? TargetPicture { get; init; }
        
        public DateTime FollowedDate { get; init; }
    }
}