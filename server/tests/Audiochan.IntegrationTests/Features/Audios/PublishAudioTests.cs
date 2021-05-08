using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Common.Builders;
using Audiochan.Core.Features.Audios.CreateAudio;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.IntegrationTests.Features.Audios
{
    [Collection(nameof(SliceFixture))]
    public class PublishAudioTests
    {
        private readonly SliceFixture _fixture;

        public PublishAudioTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Should_Publish_Audio()
        {
            // ASSIGN
            var (userId, _) = await _fixture.RunAsDefaultUserAsync();
            var audio = await new AudioBuilder()
                .AddFileName("testaudio.mp3")
                .AddFileSize(10000)
                .AddDuration(100)
                .AddUserId(userId)
                .BuildAsync();
            await _fixture.InsertAsync(audio);
            
            // ACT
            var result = await _fixture.SendAsync(new PublishAudioRequest
            {
                AudioId = audio.Id,
                Title = "Test Audio",
                Description = "This is a test audio",
                Tags = new List<string> {"apples", "oranges", "banana"},
            });

            var created = await _fixture.ExecuteDbContextAsync(database =>
            {
                return database.Audios
                    .Include(a => a.Tags)
                    .Where(a => a.Id == result.Data.Id).SingleOrDefaultAsync();
            });

            // ASSERT
            result.IsSuccess.Should().Be(true);
            result.Data.Should().NotBeNull();
            result.Data.Title.Should().Be("Test Audio");
            result.Data.Description.Should().Be("This is a test audio");
            result.Data.FileExt.Should().Be(".mp3");
            result.Data.Duration.Should().Be(100);
            result.Data.FileSize.Should().Be(10000);
            result.Data.Tags.Length.Should().Be(3);
            result.Data.Tags.Should().Contain(x => x == "apples");
            result.Data.Tags.Should().Contain(x => x == "oranges");
            result.Data.Tags.Should().Contain(x => x == "banana");
            result.Data.Author.Should().NotBeNull();
            result.Data.Author.Id.Should().Be(userId);

            created.Should().NotBeNull();
            created.Title.Should().Be("Test Audio");
            created.Description.Should().Be("This is a test audio");
            created.FileExt.Should().Be(".mp3");
            created.Duration.Should().Be(100);
            created.FileSize.Should().Be(10000);
            created.Tags.Count.Should().Be(3);
            created.Tags.Should().Contain(x => x.Name == "apples");
            created.Tags.Should().Contain(x => x.Name == "oranges");
            created.Tags.Should().Contain(x => x.Name == "banana");
            created.IsPublic.Should().BeFalse();
            created.IsPublish.Should().BeTrue();
            created.PublishDate.Should().NotBeNull();
            created.UserId.Should().Be(userId);
        }
    }
}