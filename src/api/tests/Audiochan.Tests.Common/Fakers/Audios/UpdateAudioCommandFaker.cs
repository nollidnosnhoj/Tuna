using System.Linq;
using Audiochan.Application.Features.Audios.Commands.UpdateAudio;
using Bogus;

namespace Audiochan.Tests.Common.Fakers.Audios
{
    public sealed class UpdateAudioCommandFaker : Faker<UpdateAudioCommand>
    {
        public UpdateAudioCommandFaker(long audioId)
        {
            CustomInstantiator(f =>
                new UpdateAudioCommand(audioId, 
                    f.Random.String2(3, 30), 
                    f.Lorem.Sentences(2),
                    f.Make<string>(5, _ => f.Random.String2(5, 10)).ToArray()));
        }
    }
}