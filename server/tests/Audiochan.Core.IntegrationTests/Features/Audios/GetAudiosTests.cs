using System;
using System.Threading.Tasks;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Features.Audios.CreateAudio;
using Audiochan.Core.Features.Audios.GetAudioList;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    [Collection(nameof(SliceFixture))]
    public class GetAudiosTests
    {
        private readonly SliceFixture _fixture;

        public GetAudiosTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ShouldGetAudios_BasedOnGenre()
        {
            const int resultCount = 2;
            
            await _fixture.RunAsDefaultUserAsync();
            for (var i = 0; i < 10; i++)
            {
                await _fixture.SendAsync(new CreateAudioCommand
                {
                    UploadId = UploadHelpers.GenerateUploadId(),
                    FileName = Guid.NewGuid().ToString("N") + ".mp3",
                    Duration = 100,
                    FileSize = 100,
                });
            }

            var result = await _fixture.SendAsync(new GetAudioListQuery());

            result.Should().NotBeNull();
            result.Count.Should().Be(resultCount);
            result.Items.Count.Should().Be(resultCount);
        }
    }
}