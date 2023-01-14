using System;
using Audiochan.Core.Features.Audios.Commands.CreateAudio;
using Bogus;

namespace Audiochan.Tests.Common.Fakers.Audios
{
    public sealed class CreateAudioCommandFaker : Faker<CreateAudioCommand>
    {
        public CreateAudioCommandFaker()
        {
            RuleFor(x => x.UploadId, Guid.NewGuid().ToString("N") + ".mp3");
            RuleFor(x => x.FileName, f => f.System.FileName("mp3"));
            RuleFor(x => x.FileSize, f => f.Random.Number(1, 20_000_000));
            RuleFor(x => x.Duration, f => f.Random.Number(1, 300));
            RuleFor(x => x.Title, f => f.Random.String2(3, 30));
            RuleFor(x => x.Description, f => f.Lorem.Sentences(2));
            RuleFor(x => x.Tags, f => 
                f.Make<string>(5, _ => f.Random.String2(5, 10)));
        }
    }
}