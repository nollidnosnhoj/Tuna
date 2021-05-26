using System;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    [Collection(nameof(SliceFixture))]
    public class UpdateAudioRequestTests
    {
        private readonly SliceFixture _fixture;

        public UpdateAudioRequestTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ShouldNotUpdate_WhenUserCannotModify()
        {
            // Assign
            var (ownerId, _) = await _fixture
                .RunAsUserAsync("kopacetic", Guid.NewGuid().ToString(), Array.Empty<string>());

            var audio = new AudioFaker(ownerId).Generate();

            await _fixture.InsertAsync(audio);

            // Act
            await _fixture.RunAsDefaultUserAsync();

            var command = new UpdateAudioRequestFaker(audio.Id).Generate();

            var result = await _fixture.SendAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().Be(false);
            result.ErrorCode.Should().Be(ResultError.NotFound);
        }

        [Fact]
        public async Task ShouldUpdateSuccessfully()
        {
            // Assign
            var (ownerId, _) = await _fixture
                .RunAsUserAsync("kopacetic", Guid.NewGuid().ToString(), Array.Empty<string>());

            var audio = new AudioFaker(ownerId).Generate();

            await _fixture.InsertAsync(audio);
            
            // Act
            var command = new UpdateAudioRequestFaker(audio.Id).Generate();

            var result = await _fixture.SendAsync(command);

            var created = await _fixture.ExecuteDbContextAsync(database =>
            {
                return database.Audios
                    .Include(a => a.Tags)
                    .Include(a => a.User)
                    .SingleOrDefaultAsync(a => a.Id == audio.Id);
            });

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().Be(true);
            result.Data.Should().NotBeNull();
            result.Data.Should().BeOfType<AudioDetailViewModel>();
            result.Data.Should().NotBeNull();
            result.Data.Title.Should().Be(command.Title);
            result.Data.Description.Should().Be(command.Description);
            result.Data.Tags.Count.Should().Be(command.Tags.Count);

            created.Should().NotBeNull();
            created.Title.Should().Be(command.Title);
            created.Description.Should().Be(command.Description);
            created.Tags.Count.Should().Be(command.Tags.Count);
        }
    }
}