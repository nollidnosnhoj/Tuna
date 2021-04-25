using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Features.Audios.CreateAudio;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Enums;
using Audiochan.Core.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.IntegrationTests.Features.Audios
{
    [Collection(nameof(SliceFixture))]
    public class CreateAudioTests
    {
        private readonly SliceFixture _fixture;

        public CreateAudioTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Should_Create_New_Audio()
        {
            // ASSIGN
            var (userId, _) = await _fixture.RunAsDefaultUserAsync();

            var uploadId = UploadHelpers.GenerateUploadId();

            // ACT
            var result = await _fixture.SendAsync(new CreateAudioRequest
            {
                UploadId = uploadId,
                FileName = "testaudio.mp3",
                Duration = 100,
                FileSize = 10000,
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
            created.UploadId.Should().Be(uploadId);
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
            created.PrivateKey.Should().NotBeEmpty();
            created.UserId.Should().Be(userId);
        }
    }
}