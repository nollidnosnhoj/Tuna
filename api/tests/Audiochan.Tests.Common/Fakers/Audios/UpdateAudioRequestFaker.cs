using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios.UpdateAudio;
using Bogus;

namespace Audiochan.Tests.Common.Fakers.Audios
{
    public sealed class UpdateAudioRequestFaker : Faker<UpdateAudioCommand>
    {
        public UpdateAudioRequestFaker(long audioId)
        {
            RuleFor(x => x.AudioId, audioId);
            RuleFor(x => x.Title, f => f.Random.String2(3, 30));
            RuleFor(x => x.Description, f => f.Lorem.Sentences(2));
            RuleFor(x => x.Visibility, f => f.PickRandom<Visibility>());
            RuleFor(x => x.Tags, f =>
            {
                return f.Make(3, () => f.Random.String2(5, 10)).FormatTags();
            });
        }
    }
}