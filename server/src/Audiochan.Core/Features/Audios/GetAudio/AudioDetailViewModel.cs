using System;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Models;

namespace Audiochan.Core.Features.Audios.GetAudio
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
        public DateTime Created { get; init; }
        public DateTime? LastModified { get; init; }
        public UserDto User { get; init; }
    }
}