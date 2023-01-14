using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Audiochan.Core.Features.Audios.Dtos;
using Audiochan.Core.IntegrationTests.Extensions;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    [Collection(nameof(SharedTestCollection))]
    public class CreateAudioTests
    {
        private readonly HttpClient _httpClient;
        
        public CreateAudioTests(AudiochanApiFactory factory)
        {
            _httpClient = factory.CreateClientWithTestAuth();
        }
        
        [Fact]
        public async Task SuccessfullyCreateAudio()
        {
            // Assign
            var request = new CreateAudioCommandFaker().Generate();

            // Act
            using var response = await _httpClient.PostAsJsonAsync("audios", request);
            var audio = await response.Content.ReadFromJsonAsync<AudioDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            audio.Should().NotBeNull();
            audio!.Title.Should().Be(request.Title);
            audio.Description.Should().Be(request.Description);
            audio.Duration.Should().Be(request.Duration);
            audio.Size.Should().Be(request.FileSize);
            audio.User.Should().NotBeNull();
            audio.User.Id.Should().Be(1);
            audio.User.UserName.Should().Be("testuser");
        }
    }
}