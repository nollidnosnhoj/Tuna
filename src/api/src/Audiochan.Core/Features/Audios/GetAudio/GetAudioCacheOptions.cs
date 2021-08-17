using System;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Interfaces;

namespace Audiochan.Core.Features.Audios.GetAudio
{
    public record GetAudioCacheOptions : ICacheOptions
    {
        public GetAudioCacheOptions(long audioId)
        {
            Key = CacheKeys.Audio.GetAudio(audioId);
        }
        
        public string Key { get; }
        public TimeSpan Expiration => TimeSpan.FromMinutes(30);
    }
}