using System;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.RemoveAudio;
using Audiochan.Core.UnitTests.Builders;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    [Collection(nameof(SliceFixture))]
    public class RemoveAudioTests
    {
        private readonly SliceFixture _fixture;

        public RemoveAudioTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ShouldNotRemoveAudio_WhenUserCannotModify()
        {
            // Assign
            var (ownerId, _) =
                await _fixture.RunAsUserAsync("kopacetic", Guid.NewGuid().ToString(), Array.Empty<string>());

            var audio = new AudioBuilder("testaudio.mp3", ownerId).Build();

            await _fixture.InsertAsync(audio);

            // Act
            await _fixture.RunAsDefaultUserAsync();

            var command = new RemoveAudioCommand(audio.Id);

            var result = await _fixture.SendAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().Be(false);
            result.ErrorCode.Should().Be(ResultError.Forbidden);
        }

        [Fact]
        public async Task ShouldRemoveAudio()
        {
            var (ownerId, _) = await _fixture.RunAsDefaultUserAsync();
            var audio = new AudioBuilder("testaudio.mp3", ownerId).Build();
            await _fixture.InsertAsync(audio);

            var command = new RemoveAudioCommand(audio.Id);
            var result = await _fixture.SendAsync(command);

            var created = await _fixture.FindAsync<Audio, long>(audio.Id);

            result.Should().NotBeNull();
            result.IsSuccess.Should().Be(true);

            created.Should().BeNull();
        }

        [Fact]
        public async Task ShouldRemoveFavorited_WhenAudioIsRemoved()
        {
            // Assign
            var (ownerId, _) = await _fixture.RunAsDefaultUserAsync();

            var audio = new AudioBuilder("testaudio.mp3", ownerId).Build();

            await _fixture.InsertAsync(audio);

            var (favoriterId, _) = await _fixture
                .RunAsUserAsync("kopacetic", Guid.NewGuid().ToString(), Array.Empty<string>());

            await _fixture.InsertAsync(new FavoriteAudio
            {
                AudioId = audio.Id,
                UserId = favoriterId,
                Created = DateTime.UtcNow
            });

            // Act
            await _fixture.RunAsDefaultUserAsync();
            var command = new RemoveAudioCommand(audio.Id);
            var result = await _fixture.SendAsync(command);
            var favorited = await _fixture.ExecuteDbContextAsync(database =>
            {
                return database.FavoriteAudios.SingleOrDefaultAsync(x =>
                    x.AudioId == audio.Id && x.UserId == favoriterId);
            });

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().Be(true);
            favorited.Should().BeNull();
        }
    }
}