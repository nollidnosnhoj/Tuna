using System;
using System.Collections.Generic;
using Audiochan.Core.Common.Models;

namespace Audiochan.Core.Features.Audios.GetAudio
{
    public record AudioDetailViewModel
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = null!;
        public string? Description { get; init; }
        public bool IsPublic { get; init; }
        public List<string> Tags { get; init; } = new();
        public decimal Duration { get; init; }
        public long FileSize { get; init; }
        public string FileExt { get; init; } = null!;
        public string? Picture { get; init; }
        public DateTime Created { get; init; }
        public DateTime? LastModified { get; init; }
        public string AudioUrl { get; init; } = null!;
        public MetaAuthorDto Author { get; init; } = null!;
    }
}