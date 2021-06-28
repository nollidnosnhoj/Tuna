using System;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.RemoveAudio;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    [Collection(nameof(SliceFixture))]
    public class RemoveAudioTests
    {
        private readonly SliceFixture _fixture;

        public RemoveAudioTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ShouldRemoveAudio()
        {
            var (ownerId, _) = await _fixture.RunAsDefaultUserAsync();
            var audio = new AudioFaker(ownerId).Generate();
            await _fixture.InsertAsync(audio);

            var command = new RemoveAudioCommand(audio.Id);
            var result = await _fixture.SendAsync(command);

            var created = await _fixture.FindAsync<Audio, long>(audio.Id);

            result.Should().NotBeNull();
            result.IsSuccess.Should().Be(true);

            created.Should().BeNull();
        }
    }
}