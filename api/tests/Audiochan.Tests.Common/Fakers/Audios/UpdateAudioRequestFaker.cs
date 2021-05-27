using System;
using Audiochan.API.Features.Audios.UpdateAudio;
using Audiochan.Core.Extensions;
using Bogus;

namespace Audiochan.Tests.Common.Fakers.Audios
{
    public sealed class UpdateAudioRequestFaker : Faker<UpdateAudioRequest>
    {
        public UpdateAudioRequestFaker(Guid audioId)
        {
            RuleFor(x => x.AudioId, audioId);
            RuleFor(x => x.Title, f => f.Random.String2(3, 30));
            RuleFor(x => x.Description, f => f.Lorem.Sentences(2));
            RuleFor(x => x.IsPublic, f => f.Random.Bool());
            RuleFor(x => x.Tags, f =>
            {
                return f.Make(3, () => f.Random.String2(5, 10)).FormatTags();
            });
        }
    }
}