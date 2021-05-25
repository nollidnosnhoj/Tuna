using System.Threading.Tasks;
using Audiochan.Core.Features.Audios.CreateAudio;
using Audiochan.Tests.Common.Fakers;
using Audiochan.Tests.Common.Fakers.Audios;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    [Collection(nameof(SliceFixture))]
    public class CreateAudioRequestTests
    {
        private readonly SliceFixture _fixture;

        public CreateAudioRequestTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task SuccessfullyCreateAudio()
        {
            var (adminId, adminUsername) = await _fixture.RunAsAdministratorAsync();
            var request = new CreateAudioRequestFaker().Generate();

            var response = await _fixture.SendAsync(request);

            var audio = await _fixture.ExecuteDbContextAsync(dbContext =>
            {
                return dbContext.Audios
                    .Include(a => a.Tags)
                    .Include(a => a.User)
                    .SingleOrDefaultAsync(a => a.Id == response.Data.Id);
            });

            response.Should().NotBeNull();
            response.IsSuccess.Should().BeTrue();
            response.Data.Should().NotBeNull();
            response.Data.Id.Should().NotBeEmpty();
            response.Data.Title.Should().Be(request.Title);
            response.Data.Description.Should().Be(request.Description);
            response.Data.IsPublic.Should().Be(request.IsPublic ?? false);
            response.Data.Duration.Should().Be(request.Duration);
            response.Data.FileSize.Should().Be(request.FileSize);
            response.Data.FileExt.Should().Be(".mp3");
            response.Data.Author.Should().NotBeNull();
            response.Data.Author.Id.Should().Be(adminId);
            response.Data.Author.Username.Should().Be(adminUsername);

            audio.Should().NotBeNull();
            audio.OriginalFileName.Should().Be(request.FileName);
            audio.ContentType.Should().Be(request.ContentType);
            audio.FileName.Should().Contain(request.UploadId);
            audio.FileName.Should().EndWith(".mp3");
        }
    }
}