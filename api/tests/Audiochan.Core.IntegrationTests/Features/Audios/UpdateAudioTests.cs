using System;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    [Collection(nameof(SliceFixture))]
    public class UpdateAudioTests
    {
        private readonly SliceFixture _fixture;

        public UpdateAudioTests(SliceFixture fixture)
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

            await _fixture.SendAsync(command);

            var created = await _fixture.ExecuteDbContextAsync(database =>
            {
                return database.Audios
                    .Include(a => a.Tags)
                    .Include(a => a.User)
                    .SingleOrDefaultAsync(a => a.Id == audio.Id);
            });

            // Assert
            created.Should().NotBeNull();
            created.Title.Should().Be(command.Title);
            created.Description.Should().Be(command.Description);
            created.Tags.Count.Should().Be(command.Tags!.Count);
        }
    }
}