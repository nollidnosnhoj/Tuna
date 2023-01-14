using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Audiochan.Core.IntegrationTests.Extensions;
using Audiochan.Core.Persistence;
using Audiochan.Domain.Entities;
using Audiochan.Tests.Common.Fakers.Audios;
using Audiochan.Tests.Common.Fakers.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Users
{
    public class SetFavoriteAudioTests
    {
        private readonly AudiochanApiFactory _factory;
        public SetFavoriteAudioTests(AudiochanApiFactory factory)
        {
            _factory = factory;
        }
        
        [Fact]
        public async Task AddFavoriteTest()
        {
            using var scope = _factory.Services.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            var users = new UserFaker().Generate(2);
            var target = users[0];
            var observer = users[1];

            await dbContext.Users.AddRangeAsync(target, observer);
            await dbContext.SaveChangesAsync();
            
            var audio = new AudioFaker(target.Id).Generate();
            await dbContext.Audios.AddAsync(audio);
            await dbContext.SaveChangesAsync();

            // Act
            using var client = _factory.CreateClientWithTestAuth()
                .AddUserIdToAuthorization(observer.Id);
            using var request = new HttpRequestMessage(HttpMethod.Put, $"/me/favorites/audios/{audio.Id}");
            using var response = await client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var refetchAudio = await dbContext.Audios
                    .AsNoTracking()
                    .Include(u => u.FavoriteAudios)
                    .SingleOrDefaultAsync(a => a.Id == audio.Id);

            // Assert
            refetchAudio.Should().NotBeNull();
            refetchAudio!.FavoriteAudios.Should().NotBeEmpty();
            refetchAudio.FavoriteAudios.Should().Contain(x => x.UserId == observer.Id);
        }

        [Fact]
        public async Task ShouldSuccessfullyUnfavoriteAudio()
        {
            // Assign
            using var scope = _factory.Services.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            var users = new UserFaker().Generate(2);
            var target = users[0];
            var observer = users[1];

            await dbContext.Users.AddRangeAsync(target, observer);
            await dbContext.SaveChangesAsync();
            
            var audio = new AudioFaker(target.Id).Generate();
            await dbContext.Audios.AddAsync(audio);
            await dbContext.SaveChangesAsync();

            var favoriteAudio = new FavoriteAudio
            {
                AudioId = audio.Id,
                UserId = observer.Id,
            };
            await dbContext.FavoriteAudios.AddAsync(favoriteAudio);
            await dbContext.SaveChangesAsync();

            // Act
            using var client = _factory.CreateClientWithTestAuth()
                .AddUserIdToAuthorization(observer.Id);
            using var request = new HttpRequestMessage(HttpMethod.Delete, $"/me/favorites/audios/{audio.Id}");
            using var response = await client.SendAsync(request);

            var refetchAudio = await dbContext.Audios
                .AsNoTracking()
                .Include(u => u.FavoriteAudios)
                .SingleOrDefaultAsync(a => a.Id == audio.Id);

            // Assert
            refetchAudio.Should().NotBeNull();
            refetchAudio!.FavoriteAudios.Should().BeEmpty();
        }
    }
}