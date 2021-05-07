using System;
using Audiochan.Core.Common.Models.Responses;

namespace Audiochan.Core.Features.Audios
{
    public record AudioViewModel
    {
        public string Id { get; init; }
        public string Title { get; init; }
        public bool IsPublic { get; init; }
        public int Duration { get; init; }
        public string Picture { get; init; }
        public DateTime Uploaded { get; init; }
        public string AudioUrl { get; init; }
        public MetaAuthorDto Author { get; init; }
    }
}