using System;
using System.Threading.Tasks;
using Audiochan.Core.Features.Audios.UploadAudio;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Audiochan.IntegrationTests.Features.Audios
{
    [Collection(nameof(SliceFixture))]
    public class UploadAudioRequestTest
    {
        private readonly SliceFixture _fixture;
        private readonly Randomizer _randomizer;

        public UploadAudioRequestTest(SliceFixture fixture)
        {
            _fixture = fixture;
            _randomizer = new Randomizer();
        }

        [Fact]
        public async Task SuccessfullyUploadAudio()
        {
            // assign
            var extension = ".mp3";
            var request = new UploadAudioRequest
            {
                Duration = _randomizer.Number(30, 360) + _randomizer.Decimal(),
                FileName = _randomizer.Word() + extension,
                FileSize = _randomizer.Number(1000, 10000000)
            };

            await _fixture.RunAsDefaultUserAsync();
            _fixture.SetCurrentTime(new DateTime(2021, 12, 25, 12, 0, 0));

            // act
            var response = await _fixture.SendAsync(request);

            // assert
            var audio = await _fixture.ExecuteDbContextAsync(dbContext =>
                dbContext.Audios.FindAsync(response.AudioId));
            
            response.Should().NotBeNull();
            response.AudioId.Should().NotBeEmpty();
            audio.Id.Should().Be(response.AudioId);
            audio.Duration.Should().Be(request.Duration);
            audio.OriginalFileName.Should().Be(request.FileName);
            audio.FileExt.Should().Be(extension);
            audio.FileName.Should().Be(response.AudioId + extension);
            audio.FileSize.Should().Be(request.FileSize);
        }
    }
}