using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    public class UpdateAudioTests : TestBase
    {
        public UpdateAudioTests(TestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task ShouldNotUpdate_WhenUserCannotModify()
        {
            // Assign
            var (ownerId, _) = await RunAsUserAsync("kopacetic");

            var audio = new AudioFaker(ownerId).Generate();

            InsertIntoDatabase(audio);

            // Act
            await RunAsDefaultUserAsync();

            var command = new UpdateAudioRequestFaker(audio.Id).Generate();

            var result = await SendAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().Be(false);
            result.ErrorCode.Should().Be(ResultError.Forbidden);
        }

        [Fact]
        public async Task ShouldUpdateSuccessfully()
        {
            // Assign
            var (ownerId, _) = await RunAsUserAsync("kopacetic");

            var audio = new AudioFaker(ownerId).Generate();

            InsertIntoDatabase(audio);
            
            // Act
            var command = new UpdateAudioRequestFaker(audio.Id).Generate();

            await SendAsync(command);

            var created = ExecuteDbContext(database =>
            {
                return database.Audios
                    .Include(a => a.Tags)
                    .Include(a => a.User)
                    .SingleOrDefault(a => a.Id == audio.Id);
            });

            // Assert
            created.Should().NotBeNull();
            created!.Title.Should().Be(command.Title);
            created.Description.Should().Be(command.Description);
            created.Tags.Count.Should().Be(command.Tags!.Count);
        }
        
        [Fact]
        public async Task ShouldAddSecret_WhenAudioChangedToPrivate()
        {
            // Assign
            var (ownerId, _) = await RunAsUserAsync("kopacetic");

            var audio = new AudioFaker(ownerId)
                .WithVisibility(Visibility.Public)
                .Generate();

            InsertIntoDatabase(audio);
            
            // Act
            var command = new UpdateAudioRequestFaker(audio.Id)
                .SetFixedVisibility(Visibility.Private)
                .Generate();

            await SendAsync(command);

            var created = ExecuteDbContext(database =>
            {
                return database.Audios
                    .SingleOrDefault(a => a.Id == audio.Id);
            });

            // Assert
            created.Should().NotBeNull();
            created!.Secret.Should().NotBeNullOrEmpty();
        }
        
        [Fact]
        public async Task ShouldRemoveSecret_WhenAudioChangedToPublic()
        {
            // Assign
            var (ownerId, _) = await RunAsUserAsync("kopacetic");

            var audio = new AudioFaker(ownerId)
                .WithVisibility(Visibility.Private)
                .Generate();

            InsertIntoDatabase(audio);
            
            // Act
            var command = new UpdateAudioRequestFaker(audio.Id)
                .SetFixedVisibility(Visibility.Public)
                .Generate();

            await SendAsync(command);

            var created = ExecuteDbContext(database =>
            {
                return database.Audios
                    .SingleOrDefault(a => a.Id == audio.Id);
            });

            // Assert
            created.Should().NotBeNull();
            created!.Secret.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task ShouldInvalidateCacheSuccessfully()
        {
            // Assign
            var (userId, _) = await RunAsDefaultUserAsync();
            var audio = new AudioFaker(userId).Generate();
            InsertIntoDatabase(audio);
            await SendAsync(new GetAudioQuery(audio.Id));
            var command = new UpdateAudioRequestFaker(audio.Id).Generate();
            await SendAsync(command);
            
            // Act
            var (cacheExists, _) = await GetCache<AudioDto>(CacheKeys.Audio.GetAudio(audio.Id));
            
            // Assert
            cacheExists.Should().BeFalse();
        }
    }
}