using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    public class CreateAudioTests : TestBase
    {
        public CreateAudioTests(TestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task SuccessfullyCreateAudio()
        {
            // Assign
            var (adminId, adminUsername) = await RunAsAdministratorAsync();
            var request = new CreateAudioRequestFaker().Generate();

            // Act
            var response = await SendAsync(request);
            var audio = await SendAsync(new GetAudioQuery(response.Data));

            // Assert
            response.Data.Should().NotBeEmpty();
            audio.Should().NotBeNull();
            audio!.Title.Should().Be(request.Title);
            audio.Description.Should().Be(request.Description);
            audio.Visibility.Should().Be(request.Visibility);
            audio.Duration.Should().Be(request.Duration);
            audio.Size.Should().Be(request.FileSize);
            audio.User.Should().NotBeNull();
            audio.User.Id.Should().Be(adminId);
            audio.User.Username.Should().Be(adminUsername);
        }
        
        [Fact]
        public async Task ShouldCreateCacheSuccessfully()
        {
            // Assign
            await RunAsAdministratorAsync();
            var request = new CreateAudioRequestFaker().Generate();
            var response = await SendAsync(request);
            await SendAsync(new GetAudioQuery(response.Data)); // trigger the caching
            
            // Act
            var (exists, audio) =
                await GetCache<AudioViewModel>(CacheKeys.Audio.GetAudio(response.Data));

            // Assert
            exists.Should().BeTrue();
            audio.Should().NotBeNull();
            audio.Should().BeOfType<AudioViewModel>();
        }
    }
}