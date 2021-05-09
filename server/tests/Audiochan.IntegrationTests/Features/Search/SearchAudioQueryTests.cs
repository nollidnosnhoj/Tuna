using System.Collections.Generic;
using System.Threading.Tasks;
using Audiochan.Core.Common.Builders;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Features.Audios.SearchAudios;
using Audiochan.UnitTests;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Audiochan.IntegrationTests.Features.Search
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
            var dateTimeProvider = await _fixture.ExecuteScopeAsync(sp => 
                Task.FromResult(sp.GetRequiredService<IDateTimeProvider>()));
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
                var audio = await new AudioBuilder()
                    .UseTestDefaults(userId)
                    .AddTitle(title)
                    .SetPublic(true)
                    .SetPublishToTrue(dateTimeProvider.Now)
                    .BuildAsync();
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
            
            var dateTimeProvider = await _fixture.ExecuteScopeAsync(sp => 
                Task.FromResult(sp.GetRequiredService<IDateTimeProvider>()));

            var (userId, _) = await _fixture.RunAsDefaultUserAsync();

            for (var i = 0; i < 10; i++)
            {
                var tags = new List<string>();

                if (i > 0 && i % 2 == 0)
                    tags.Add("testtag1");
                if (i > 0 && i % 3 == 0)
                    tags.Add("testtag2");

                var tagEntities = await _fixture.ExecuteScopeAsync(sp =>
                {
                    var tagRepository = sp.GetRequiredService<ITagRepository>();
                    return tagRepository.GetListAsync(tags);
                });

                var audio = await new AudioBuilder()
                    .AddFileName("test.mp3")
                    .AddTitle($"Test Song #{i + 1}")
                    .AddDuration(100)
                    .AddFileSize(100)
                    .AddTags(tagEntities)
                    .SetPublishToTrue(dateTimeProvider.Now)
                    .AddUserId(userId)
                    .BuildAsync();

                await _fixture.InsertAsync(audio);
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