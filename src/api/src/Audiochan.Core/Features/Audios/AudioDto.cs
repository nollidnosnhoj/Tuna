using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Audiochan.Core.Common.Converters.Json;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Models;
using Audiochan.Domain.Abstractions;

namespace Audiochan.Core.Features.Audios
{
    public record AudioDto : IHasId<long>
    {
        public long Id { get; init; }
        public string Title { get; init; } = null!;
        public string Slug => HashIdHelper.EncodeLong(Id);
        public string Description { get; init; } = string.Empty;
        public List<string> Tags { get; init; } = new();
        public decimal Duration { get; init; }
        public long Size { get; init; }
        public string? Picture { get; init; }
        public bool? IsFavorited { get; init; }
        public DateTime Created { get; init; }
        public DateTime? LastModified { get; init; }
        [JsonConverter(typeof(AudioUrlJsonConverter))]
        public string Audio { get; init; } = null!;
        public MetaAuthorDto User { get; init; } = null!;
    }
}