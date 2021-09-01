using System.Collections.Generic;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios;

namespace Audiochan.Core.Features.Playlists
{
    public record PlaylistDto : IResourceDto<long>
    {
        public long Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string? Picture { get; init; }
        public List<string> Tags { get; init; } = new();
        public List<AudioDto> Audios { get; init; } = new();
        public MetaAuthorDto User { get; init; } = null!;
    }
}