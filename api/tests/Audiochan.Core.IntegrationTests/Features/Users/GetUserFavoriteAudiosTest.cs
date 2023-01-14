using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Audiochan.Common.Dtos;
using Audiochan.Core.Features.Audios.Dtos;
using Audiochan.Core.IntegrationTests.Extensions;
using Audiochan.Core.Persistence;
using Audiochan.Domain.Entities;
using Audiochan.Tests.Common.Fakers.Audios;
using Audiochan.Tests.Common.Fakers.Users;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Users
{
    public class GetUserFavoriteAudiosTest
    {
        private readonly AudiochanApiFactory _factory;
        public GetUserFavoriteAudiosTest(AudiochanApiFactory factory)
        {
            _factory = factory;
        }
        
        [Fact]
        public async Task ShouldReturnFavoriteAudioSuccessfully()
        {
            // Assign
            using var scope = _factory.Services.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            var users = new UserFaker().Generate(2);
            var target = users[0];
            var observer = users[1];

            await dbContext.Users.AddRangeAsync(target, observer);
            await dbContext.SaveChangesAsync();
            
            var audios = new AudioFaker(target.Id).Generate(3);
            await dbContext.Audios.AddRangeAsync(audios);
            await dbContext.SaveChangesAsync();

            var favoriteAudios = new List<FavoriteAudio>();
            foreach (var audio in audios)
            {
                var favoriteAudio = new FavoriteAudio
                {
                    AudioId = audio.Id,
                    UserId = observer.Id,
                };
                
                favoriteAudios.Add(favoriteAudio);
            }

            await dbContext.FavoriteAudios.AddRangeAsync(favoriteAudios);
            await dbContext.SaveChangesAsync();

            // Act
            using var client = _factory.CreateClientWithTestAuth()
                .AddUserIdToAuthorization(observer.Id);
            using var response = await client.GetAsync("me/favorites/audios/");
            var result = await response.Content.ReadFromJsonAsync<OffsetPagedListDto<AudioDto>>();

            // Assert
            result.Should().NotBeNull();
            result!.Items.Count.Should().Be(3);
        }
    }
}