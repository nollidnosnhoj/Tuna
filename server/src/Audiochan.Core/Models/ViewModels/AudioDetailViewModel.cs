using System;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Enums;
using Audiochan.Core.Models.Interfaces;
using Audiochan.Core.Models.Responses;

namespace Audiochan.Core.Models.ViewModels
{
    public record AudioDetailViewModel
    {
        public long Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public Visibility Visibility { get; init; }
        public string PrivateKey { get; init; }
        public string[] Tags { get; init; }
        public int Duration { get; init; }
        public long FileSize { get; init; }
        public string FileExt { get; init; }
        public string Picture { get; init; }
        public DateTime Uploaded { get; init; }
        public DateTime? LastModified { get; init; }
        public string AudioUrl { get; init; }
        public MetaAuthorDto Author { get; init; }
    }
}