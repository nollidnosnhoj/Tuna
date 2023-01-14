using System.Net;
using System.Threading.Tasks;
using Audiochan.Core.IntegrationTests.Extensions;
using Audiochan.Core.Persistence;
using Audiochan.Tests.Common.Fakers.Audios;
using Audiochan.Tests.Common.Fakers.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    
    [Collection(nameof(SharedTestCollection))]
    public class RemoveAudioTests
    {
        private readonly AudiochanApiFactory _factory;

        public RemoveAudioTests(AudiochanApiFactory factory)
        {
            _factory = factory;
        }
        
        [Fact]
        public async Task ShouldRemoveAudio()
        {
            using var scope = _factory.Services.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
            var user = new UserFaker().Generate();
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
            
            var audio = new AudioFaker(user.Id).Generate();
            await dbContext.Audios.AddAsync(audio);
            await dbContext.SaveChangesAsync();

            using var client = _factory.CreateClientWithTestAuth()
                .AddUserIdToAuthorization(user.Id);

            using var response = await client.DeleteAsync($"audios/{audio.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var result = await dbContext.Audios
                .SingleOrDefaultAsync(x => x.Id == audio.Id);

            result.Should().BeNull();
        }

        [Fact]
        public async Task ShouldNotRemoveAudio_WhenNotTheAuthor()
        {
            using var scope = _factory.Services.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
            var user = new UserFaker().Generate();
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
            
            var audio = new AudioFaker(user.Id).Generate();
            await dbContext.Audios.AddAsync(audio);
            await dbContext.SaveChangesAsync();
            
            using var client = _factory.CreateClientWithTestAuth()
                .AddUserIdToAuthorization(11111);

            using var response = await client.DeleteAsync($"audios/{audio.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}