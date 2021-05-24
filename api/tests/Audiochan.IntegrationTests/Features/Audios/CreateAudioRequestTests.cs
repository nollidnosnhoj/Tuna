using Bogus;
using Xunit;

namespace Audiochan.IntegrationTests.Features.Audios
{
    [Collection(nameof(SliceFixture))]
    public class CreateAudioRequestTests
    {
        private readonly SliceFixture _fixture;
        private readonly Randomizer _randomizer;

        public CreateAudioRequestTests(SliceFixture fixture)
        {
            _fixture = fixture;
            _randomizer = new Randomizer();
        }
    }
}