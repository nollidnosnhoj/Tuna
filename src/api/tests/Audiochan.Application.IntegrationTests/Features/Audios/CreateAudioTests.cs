using System.Threading.Tasks;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Features.Audios.Models;
using Audiochan.Application.Features.Audios.Queries.GetAudio;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using NUnit.Framework;

namespace Audiochan.Application.IntegrationTests.Features.Audios
{
    using static TestFixture;
    
    public class CreateAudioTests : TestBase
    {
        [Test]
        public async Task SuccessfullyCreateAudio()
        {
            // Assign
            var user = await RunAsDefaultUserAsync();
            user.TryGetUserId(out var userId);
            user.TryGetUserName(out var userName);
            var request = new CreateAudioCommandFaker().Generate();

            // Act
            var response = await SendAsync(request);

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Data.Should().NotBeNull();
            response.Data!.Title.Should().Be(request.Title);
            response.Data.Description.Should().Be(request.Description);
            response.Data.Duration.Should().Be(request.Duration);
            response.Data.Size.Should().Be(request.FileSize);
            response.Data.User.Should().NotBeNull();
            response.Data.User.Id.Should().Be(userId);
            response.Data.User.UserName.Should().Be(userName);
        }

        // [Test]
        // public async Task ShouldCreateCacheSuccessfully()
        // {
        //     // Assign
        //     await RunAsDefaultUserAsync();
        //     var request = new CreateAudioCommandFaker()
        //         .Generate();
        //     var response = await SendAsync(request);
        //     
        //     // Act
        //     var audio = await GetCache<AudioDto>(CacheKeys.Audio.GetAudio(response.Data));
        //
        //     // Assert
        //     audio.Should().NotBeNull();
        //     audio.Should().BeOfType<AudioDto>();
        // }
    }
}