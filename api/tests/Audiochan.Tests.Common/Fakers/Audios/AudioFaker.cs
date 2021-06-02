using System;
using Audiochan.Core.Entities;
using Bogus;

namespace Audiochan.Tests.Common.Fakers.Audios
{
    public sealed class AudioFaker : Faker<Audio>
    {
        public AudioFaker(string userId)
        {
            RuleFor(x => x.UserId, userId);
            RuleFor(x => x.Title, f => f.Random.String2(3, 30));
            RuleFor(x => x.Description, f => f.Lorem.Sentences(2));
            RuleFor(x => x.IsPublic, f => f.Random.Bool());
            RuleFor(x => x.FileSize, f => f.Random.Number(1, 20_000_000));
            RuleFor(x => x.ContentType, "audio/mp3");
            RuleFor(x => x.FileExt, ".mp3");
            RuleFor(x => x.Duration, f => f.Random.Number(1, 300));
            RuleFor(x => x.BlobName, f => f.Random.String2(12) + ".mp3");
            RuleFor(x => x.FileName, f => f.System.FileName("mp3"));
            RuleFor(x => x.Tags, f => f.Make(f.Random.Number(1, 5), () => 
                    new Tag {Name = f.Random.String2(5, 10)}));
        }
    }
}