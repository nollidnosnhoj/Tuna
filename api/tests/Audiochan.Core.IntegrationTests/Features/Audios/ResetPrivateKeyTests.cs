using System.Threading.Tasks;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios.ResetPrivateKey;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    [Collection(nameof(SliceFixture))]
    public class ResetPrivateKeyTests
    {
        private readonly SliceFixture _sliceFixture;

        public ResetPrivateKeyTests(SliceFixture sliceFixture)
        {
            _sliceFixture = sliceFixture;
        }

        [Fact]
        public async Task ShouldResetPrivateKey()
        {
            var (ownerId, _) = await _sliceFixture.RunAsDefaultUserAsync();
            var audio = new AudioFaker(ownerId).Generate();
            audio.Visibility = Visibility.Private;
            await _sliceFixture.InsertAsync(audio);

            var result = await _sliceFixture.SendAsync(new ResetPrivateKeyCommand(audio.Id));

            audio.Secret.Should().NotBeNullOrEmpty();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNullOrEmpty();
            result.Data.Should().NotBe(audio.Secret);
        }
    }
}