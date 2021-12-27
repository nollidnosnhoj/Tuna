using System.Linq;
using System.Threading.Tasks;
using Audiochan.Application.Commons.Exceptions;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Audiochan.Application.IntegrationTests.Features.Audios
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

            await FluentActions.Awaiting(() => SendAsync(command))
                .Should().ThrowAsync<ForbiddenException>();
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
            created.Tags.Length.Should().Be(command.Tags!.Length);
        }
    }
}