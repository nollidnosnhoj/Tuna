using System.Collections.Generic;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Features.Audios.CreateAudio;
using Audiochan.Core.Features.Search;
using Audiochan.Core.Features.Search.SearchAudios;
using Audiochan.Core.UnitTests.Builders;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Search
{
    [Collection(nameof(SliceFixture))]
    public class SearchAudioQueryTests
    {
        private readonly SliceFixture _fixture;

        public SearchAudioQueryTests(SliceFixture sliceFixture)
        {
            _fixture = sliceFixture;
        }

        [Fact]
        public async Task ShouldFilterAudio_BasedOnSearchTerm()
        {
            var random = new Randomizer();
            var (userId, _) = await _fixture.RunAsDefaultUserAsync();

            for (var i = 0; i < 10; i++)
            {
                var title = "testaudio" + i;
                if (i > 0 && i % 3 == 0)
                {
                    title = "EXAMPLE";
                    if (random.Int(1, 10) % 2 == 0)
                        title = "ABC123 " + title;
                    if (random.Int(1, 10) % 2 == 0)
                        title += " ABC123";
                }
                var audio = new AudioBuilder(userId)
                    .Title(title)
                    .Publicity(Visibility.Public)
                    .Build();
                await _fixture.InsertAsync(audio);
            }

            var result = await _fixture.SendAsync(new SearchAudiosRequest
            {
                Q = "example"
            });

            result.Should().NotBeNull();
            result.Count.Should().Be(3);
            result.Items.Count.Should().Be(3);
        }

        [Fact]
        public async Task ShouldFilterAudio_BasedOnTags()
        {
            const int resultCount = 6;

            await _fixture.RunAsDefaultUserAsync();

            for (var i = 0; i < 10; i++)
            {
                var tags = new List<string>();

                if (i > 0 && i % 2 == 0)
                    tags.Add("testtag1");
                if (i > 0 && i % 3 == 0)
                    tags.Add("testtag2");

                await _fixture.SendAsync(new CreateAudioRequest
                {
                    UploadId = UploadHelpers.GenerateUploadId(),
                    FileName = "test.mp3",
                    Duration = 100,
                    FileSize = 100,
                    Tags = tags,
                    Publicity = "public"
                });
            }

            var result = await _fixture.SendAsync(new SearchAudiosRequest
            {
                Tags = "testtag1, testtag2"
            });

            result.Should().NotBeNull();
            result.Count.Should().Be(resultCount);
            result.Items.Count.Should().Be(resultCount);
        }
    }
}