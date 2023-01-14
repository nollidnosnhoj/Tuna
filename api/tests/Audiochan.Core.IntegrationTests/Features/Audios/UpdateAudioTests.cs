using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Audiochan.Core.Features.Audios.Dtos;
using Audiochan.Core.IntegrationTests.Extensions;
using Audiochan.Core.Persistence;
using Audiochan.Tests.Common.Fakers.Audios;
using Audiochan.Tests.Common.Fakers.Users;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{

    [Collection(nameof(SharedTestCollection))]
    public class UpdateAudioTests
    {
        private readonly AudiochanApiFactory _factory;

        public UpdateAudioTests(AudiochanApiFactory factory)
        {
            _factory = factory;
        }
        
        [Fact]
        public async Task ShouldNotUpdate_WhenUserCannotModify()
        {
            using var scope = _factory.Services.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
            var user = new UserFaker().Generate();
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
            
            var audio = new AudioFaker(user.Id).Generate();
            await dbContext.Audios.AddAsync(audio);
            await dbContext.SaveChangesAsync();

            var request = new UpdateAudioCommandFaker(audio.Id).Generate();
            using var client = _factory.CreateClientWithTestAuth().AddUserIdToAuthorization(11111111);
            using var response = await client.PutAsJsonAsync($"audios/${audio.Id}", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task ShouldUpdateSuccessfully()
        {
            using var scope = _factory.Services.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
            var user = new UserFaker().Generate();
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
            
            var audio = new AudioFaker(user.Id).Generate();
            await dbContext.Audios.AddAsync(audio);
            await dbContext.SaveChangesAsync();

            var request = new UpdateAudioCommandFaker(audio.Id).Generate();
            using var client = _factory.CreateClientWithTestAuth().AddUserIdToAuthorization(user.Id);
            using var response = await client.PutAsJsonAsync($"audios/${audio.Id}", request);
            var result = await response.Content.ReadFromJsonAsync<AudioDto>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Assert
            result.Should().NotBeNull();
            result!.Title.Should().Be(request.Title);
            result.Description.Should().Be(request.Description);
            result.Tags.Count.Should().Be(request.Tags!.Count);
        }
    }
}