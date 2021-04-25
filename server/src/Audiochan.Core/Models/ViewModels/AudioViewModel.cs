using System;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Enums;
using Audiochan.Core.Models.Interfaces;
using Audiochan.Core.Models.Responses;

namespace Audiochan.Core.Models.ViewModels
{
    public record AudioViewModel
    {
        public long Id { get; init; }
        public string Title { get; init; }
        public Visibility Visibility { get; init; }
        public int Duration { get; init; }
        public string Picture { get; init; }
        public DateTime Uploaded { get; init; }
        public string AudioUrl { get; init; }
        public MetaAuthorDto Author { get; init; }
    }
}