using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.RemoveAudio;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    public class RemoveAudioTests : TestBase
    {
        public RemoveAudioTests(TestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task ShouldRemoveAudio()
        {
            var (ownerId, _) = await RunAsDefaultUserAsync();
            var audio = new AudioFaker(ownerId).Generate();
            InsertIntoDatabase(audio);

            var command = new RemoveAudioCommand(audio.Id);
            var result = await SendAsync(command);

            var created = ExecuteDbContext(dbContext => 
                dbContext.Audios.SingleOrDefault(x => x.Id == audio.Id));

            result.Should().NotBeNull();
            result.IsSuccess.Should().Be(true);

            created.Should().BeNull();
        }

        [Fact]
        public async Task ShouldNotRemoveAudio_WhenNotTheAuthor()
        {
            var (ownerId, _) = await RunAsDefaultUserAsync();
            var audio = new AudioFaker(ownerId).Generate();
            InsertIntoDatabase(audio);
            await RunAsUserAsync();
            var result = await SendAsync(new RemoveAudioCommand(audio.Id));

            result.IsSuccess.Should().BeFalse();
            result.ErrorCode.Should().Be(ResultError.Forbidden);
        }
    }
}