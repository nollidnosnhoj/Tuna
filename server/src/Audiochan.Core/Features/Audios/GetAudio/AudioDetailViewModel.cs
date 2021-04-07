using System;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;

namespace Audiochan.Core.Features.Audios.GetAudio
{
    public record AudioDetailViewModel : IBaseViewModel
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