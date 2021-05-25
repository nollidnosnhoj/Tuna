using System;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Features.Audios.CreateAudio;
using Bogus;

namespace Audiochan.Tests.Common.Fakers
{
    public static class AudioFakers
    {
        public static Faker<CreateAudioRequest> CreateAudioRequestFaker()
        {
            return new Faker<CreateAudioRequest>()
                .RuleFor(x => x.UploadId, () => Guid.NewGuid().ToString("N") + ".mp3")
                .RuleFor(x => x.FileName, f => f.System.FileName("mp3"))
                .RuleFor(x => x.FileSize, f => f.Random.Number(1, 20_000_000))
                .RuleFor(x => x.ContentType, () => "audio/mp3")
                .RuleFor(x => x.Duration, f => f.Random.Number(1, 300))
                .RuleFor(x => x.Title, f => f.Random.String2(3, 30))
                .RuleFor(x => x.Description, f => f.Lorem.Sentences(2))
                .RuleFor(x => x.IsPublic, f => f.Random.Bool())
                .RuleFor(x => x.Tags, f => f.Random.WordsArray(f.Random.Number(1, 5)).FormatTags());
        }
    }
}