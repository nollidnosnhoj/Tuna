﻿using System.Collections.Generic;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities.Enums;

namespace Audiochan.Core.Features.Playlists.GetPlaylistDetail
{
    public record PlaylistViewModel : IResourceModel<long>
    {
        public long Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string? Picture { get; init; }
        public Visibility Visibility { get; init; }
        public List<string> Tags { get; init; } = new();
        public MetaAuthorDto User { get; init; } = null!;
    }
}