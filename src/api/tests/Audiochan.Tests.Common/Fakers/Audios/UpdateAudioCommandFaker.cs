using Audiochan.Core.Audios;
using Bogus;

namespace Audiochan.Tests.Common.Fakers.Audios
{
    public sealed class UpdateAudioCommandFaker : Faker<UpdateAudioCommand>
    {
        public UpdateAudioCommandFaker(long audioId)
        {
            RuleFor(x => x.AudioId, audioId);
            RuleFor(x => x.Title, f => f.Random.String2(3, 30));
            RuleFor(x => x.Description, f => f.Lorem.Sentences(2));
            RuleFor(x => x.Tags, f => 
                f.Make<string>(5, _ => f.Random.String2(5, 10)));
        }
    }
}