using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Commons;
using Audiochan.Core.Commons.Extensions;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Features.Audios.Queries.GetAudio;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    using static TestFixture;

    public class UpdateAudioTests : TestBase
    {
        [Test]
        public async Task ShouldNotUpdate_WhenUserCannotModify()
        {
            // Assign
            var owner = await RunAsUserAsync("kopacetic");
            owner.TryGetUserId(out var ownerId);

            var audio = new AudioFaker(ownerId).Generate();

            InsertIntoDatabase(audio);

            // Act
            await RunAsDefaultUserAsync();

            var command = new UpdateAudioCommandFaker(audio.Id).Generate();

            var result = await SendAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().Be(false);
            result.ErrorCode.Should().Be(ResultError.Forbidden);
        }

        [Test]
        public async Task ShouldUpdateSuccessfully()
        {
            // Assign
            var owner = await RunAsUserAsync("kopacetic");
            owner.TryGetUserId(out var ownerId);

            var audio = new AudioFaker(ownerId).Generate();

            InsertIntoDatabase(audio);
            
            // Act
            var command = new UpdateAudioCommandFaker(audio.Id).Generate();

            await SendAsync(command);

            var created = ExecuteDbContext(database =>
            {
                return database.Audios
                    .Include(a => a.User)
                    .SingleOrDefault(a => a.Id == audio.Id);
            });

            // Assert
            created.Should().NotBeNull();
            created!.Title.Should().Be(command.Title);
            created.Description.Should().Be(command.Description);
            created.Tags.Count.Should().Be(command.Tags!.Count);
        }

        [Test]
        public async Task ShouldInvalidateCacheSuccessfully()
        {
            // Assign
            var user = await RunAsDefaultUserAsync();
            user.TryGetUserId(out var userId);
            var audio = new AudioFaker(userId).Generate();
            InsertIntoDatabase(audio);
            await SendAsync(new GetAudioQuery(audio.Id));
            var command = new UpdateAudioCommandFaker(audio.Id).Generate();
            await SendAsync(command);
            
            // Act
            var cacheResult = await GetCache<AudioDto>(CacheKeys.Audio.GetAudio(audio.Id));
            
            // Assert
            cacheResult.Should().BeNull();
        }
    }
}