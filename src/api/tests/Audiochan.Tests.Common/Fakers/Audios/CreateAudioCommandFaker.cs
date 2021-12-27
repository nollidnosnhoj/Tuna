using System;
using System.Linq;
using Audiochan.Application.Features.Audios.Commands.CreateAudio;
using Bogus;

namespace Audiochan.Tests.Common.Fakers.Audios
{
    public sealed class CreateAudioCommandFaker : Faker<CreateAudioCommand>
    {
        public CreateAudioCommandFaker()
        {
            CustomInstantiator(faker => 
                new CreateAudioCommand(
                    Guid.NewGuid().ToString("N") + ".mp3",
                    faker.Random.String2(3, 30),
                    faker.Lorem.Sentences(2),
                    faker.Make<string>(5, _ => faker.Random.String2(5, 10)).ToArray(),
                    faker.System.FileName("mp3"),
                    faker.Random.Number(1, 20_000_000),
                    faker.Random.Number(1, 30)));
        }
    }
}