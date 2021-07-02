using System.IO;
using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    [Collection(nameof(SliceFixture))]
    public class CreateAudioTests
    {
        private readonly SliceFixture _fixture;

        public CreateAudioTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task SuccessfullyCreateAudio()
        {
            // Assign
            var (adminId, adminUsername) = await _fixture.RunAsAdministratorAsync();
            var request = new CreateAudioRequestFaker().Generate();

            // Act
            var response = await _fixture.SendAsync(request);
            var audio = await _fixture.SendAsync(new GetAudioQuery(response.Data));

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
            audio.User.UserName.Should().Be(adminUsername);
        }
        
        [Fact]
        public async Task ShouldCreateCacheSuccessfully()
        {
            // Assign
            await _fixture.RunAsAdministratorAsync();
            var request = new CreateAudioRequestFaker().Generate();
            var response = await _fixture.SendAsync(request);
            await _fixture.SendAsync(new GetAudioQuery(response.Data)); // trigger the caching
            
            // Act
            var (exists, audio) =
                await _fixture.GetCache<AudioDetailViewModel>(CacheKeys.Audio.GetAudio(response.Data));

            // Assert
            exists.Should().BeTrue();
            audio.Should().NotBeNull();
            audio.Should().BeOfType<AudioDetailViewModel>();
        }
    }
}