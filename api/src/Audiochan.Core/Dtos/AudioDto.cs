using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Audiochan.Core.Converters.Json;
using Audiochan.Core.Interfaces;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Dtos
{
    public record AudioDto : IHasId<long>, IMapFrom<Audio>
    {
        public long Id { get; set; }
        
        public string Title { get; set; } = null!;
        
        public string Slug { get; set; } = null!;
        
        public string Description { get; set; } = string.Empty;
        
        public List<string> Tags { get; set; } = new();
        
        public decimal Duration { get; set; }
        
        public long Size { get; set; }
        
        [JsonConverter(typeof(AudioPictureJsonConverter))]
        public string? Picture { get; set; }
        
        public bool? IsFavorited { get; set; }
        
        public DateTime Created { get; set; }
        
        public DateTime? LastModified { get; set; }
        
        [JsonConverter(typeof(AudioStreamLinkJsonConverter))]
        public string Src { get; set; } = null!;
        
        public UserDto User { get; set; } = null!;
    }
}