using System;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Features.Audios.CreateAudio;
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
        private readonly Faker<CreateAudioRequest> _faker;

        public CreateAudioRequestTests(SliceFixture fixture)
        {
            _fixture = fixture;
            _faker = new Faker<CreateAudioRequest>()
                .RuleFor(x => x.UploadId, () => Guid.NewGuid().ToString("N") + ".mp3")
                .RuleFor(x => x.FileName, f => f.System.FileName("mp3"))
                .RuleFor(x => x.FileSize, f => f.Random.Number(1, 20_000_000))
                .RuleFor(x => x.ContentType, () => "audio/mp3")
                .RuleFor(x => x.Duration, f => f.Random.Number(1, 300))
                .RuleFor(x => x.Title, f => f.Random.String2(3, 30))
                .RuleFor(x => x.Description, f => f.Lorem.Sentences(2))
                .RuleFor(x => x.IsPublic, f => f.Random.Bool())
                .RuleFor(x => x.Tags, f => f.Random.WordsArray(f.Random.Number(1, 5)).FormatTags());
        }

        [Fact]
        public async Task SuccessfullyCreateAudio()
        {
            var (adminId, adminUsername) = await _fixture.RunAsAdministratorAsync();
            var request = _faker.Generate();

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