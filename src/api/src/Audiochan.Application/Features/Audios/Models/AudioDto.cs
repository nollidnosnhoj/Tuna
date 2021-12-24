using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Audiochan.Application.Commons.Converters.Json;
using Audiochan.Application.Commons.Interfaces;
using Audiochan.Application.Features.Users.Models;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Entities;

namespace Audiochan.Application.Features.Audios.Models
{
    public record AudioDto : IHasId<long>, IMapFrom<Audio>
    {
        public long Id { get; init; }
        
        public string Title { get; init; } = null!;
        
        public string Slug { get; init; } = null!;
        
        public string Description { get; init; } = string.Empty;
        
        public List<string> Tags { get; init; } = new();
        
        public decimal Duration { get; init; }
        
        public long Size { get; init; }
        
        [JsonConverter(typeof(AudioPictureJsonConverter))]
        public string? Picture { get; init; }
        
        public bool? IsFavorited { get; init; }
        
        public DateTime Created { get; init; }
        
        public DateTime? LastModified { get; init; }
        
        [JsonConverter(typeof(AudioStreamLinkJsonConverter))]
        public string Src { get; init; } = null!;
        
        public UserDto User { get; init; } = null!;
    }
}