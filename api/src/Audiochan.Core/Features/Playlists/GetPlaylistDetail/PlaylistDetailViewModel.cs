using System;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities.Enums;

namespace Audiochan.Core.Features.Playlists.GetPlaylistDetail
{
    public record PlaylistDetailViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Picture { get; set; }
        public Visibility Visibility { get; set; }
        public MetaAuthorDto User { get; set; } = null!;
    }
}