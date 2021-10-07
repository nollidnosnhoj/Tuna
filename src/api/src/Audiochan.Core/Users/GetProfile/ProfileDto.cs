﻿using System.Text.Json.Serialization;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Converters.Json;
using Audiochan.Core.Common.Mappings;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Core.Users.GetProfile
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

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, ProfileDto>();
        }
    }
}