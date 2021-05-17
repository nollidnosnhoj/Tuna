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
                FileName = _randomizer.Word() + extension,
                FileSize = _randomizer.Number(1000, 10000000)
            };

            await _fixture.RunAsDefaultUserAsync();
            _fixture.SetCurrentTime(new DateTime(2021, 12, 25, 12, 0, 0));

            // act
            var response = await _fixture.SendAsync(request);

            // assert
            var audio = await _fixture.ExecuteDbContextAsync(dbContext =>
                dbContext.Audios.FindAsync(response.UploadId));
            
            response.Should().NotBeNull();
            response.UploadId.Should().NotBeEmpty();
            audio.Id.Should().Be(response.UploadId);
            audio.OriginalFileName.Should().Be(request.FileName);
            audio.FileExt.Should().Be(extension);
            audio.FileName.Should().Be(response.UploadId + extension);
            audio.FileSize.Should().Be(request.FileSize);
        }
    }
}