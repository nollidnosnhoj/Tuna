using System;
using System.Collections.Generic;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities.Enums;

namespace Audiochan.Core.Features.Audios.GetAudio
{
    public record AudioViewModel : IResourceModel<long>
    {
        public long Id { get; init; }
        public string Title { get; init; } = null!;
        public string Slug { get; init; } = null!;
        public string Description { get; init; } = string.Empty;
        public Visibility Visibility { get; init; }
        public string? Secret { get; init; }
        public List<string> Tags { get; init; } = new();
        public decimal Duration { get; init; }
        public long Size { get; init; }
        public string? Picture { get; init; }
        public bool? IsFavorited { get; init; }
        public DateTime Created { get; init; }
        public DateTime? LastModified { get; init; }
        public string AudioUrl { get; init; } = null!;
        public MetaAuthorDto User { get; init; } = null!;
    }
}