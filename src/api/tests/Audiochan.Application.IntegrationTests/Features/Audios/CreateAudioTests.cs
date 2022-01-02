using System.Threading.Tasks;
using Audiochan.Application.Commons.Extensions;
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
            response.Should().NotBeNull();
            response.Title.Should().Be(request.Title);
            response.Description.Should().Be(request.Description);
            response.Duration.Should().Be(request.Duration);
            response.Size.Should().Be(request.FileSize);
            response.User.Should().NotBeNull();
            response.User.Id.Should().Be(userId);
            response.User.UserName.Should().Be(userName);
        }
    }
}