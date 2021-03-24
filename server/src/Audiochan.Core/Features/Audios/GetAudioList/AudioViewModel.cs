using System;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Options;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Features.Audios.GetAudioList
{
    public class AudioViewModel
    {
        public long Id { get; init; }
        public string Title { get; init; }
        public Visibility Visibility { get; init; }
        public int Duration { get; init; }
        public string Picture { get; init; }
        public DateTime Created { get; init; }
        public string AudioUrl { get; init; }
        public MetaUserDto User { get; init; }
        
        public static AudioViewModel MapFrom(Audio audio, AudiochanOptions options) =>
            AudioMappingExtensions.AudioToListProjection(options).Compile().Invoke(audio);
    }
}