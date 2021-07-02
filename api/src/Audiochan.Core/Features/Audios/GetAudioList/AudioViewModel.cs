using System;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities.Enums;

namespace Audiochan.Core.Features.Audios.GetAudioList
{
    public record AudioViewModel
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = null!;
        public Visibility Visibility { get; init; }
        public decimal Duration { get; init; }
        public string? Picture { get; init; }
        public DateTime Created { get; init; }
        public string AudioUrl { get; init; } = null!;
        public MetaAuthorDto User { get; init; } = null!;
    }
}