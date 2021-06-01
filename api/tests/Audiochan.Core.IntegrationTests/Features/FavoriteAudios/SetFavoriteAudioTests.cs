using System;
using System.Threading.Tasks;
using Audiochan.API.Features.FavoriteAudios.SetFavorite;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.FavoriteAudios
{
    [Collection(nameof(SliceFixture))]
    public class SetFavoriteAudioTests
    {
        private readonly SliceFixture _sliceFixture;

        public SetFavoriteAudioTests(SliceFixture sliceFixture)
        {
            _sliceFixture = sliceFixture;
        }
        
        [Fact]
        public async Task AddFavoriteTest()
        {
            var (targetId, _) = await _sliceFixture.RunAsDefaultUserAsync();
            
            var (observerId, _) =
                await _sliceFixture.RunAsUserAsync("kopacetic", "kopacetic123!", Array.Empty<string>());

            var audio = new AudioFaker(targetId).Generate();
            await _sliceFixture.InsertAsync(audio);

            await _sliceFixture.SendAsync(new SetFavoriteAudioRequest(audio.Id, observerId, true));

            var refetchAudio = await _sliceFixture.ExecuteDbContextAsync(database =>
            {
                return database.Audios
                    .AsNoTracking()
                    .Include(u => u.Favorited)
                    .SingleOrDefaultAsync(a => a.Id == audio.Id);
            });

            refetchAudio.Favorited.Should().NotBeEmpty();
            refetchAudio.Favorited.Should().Contain(x => x.UserId == observerId && x.AudioId == audio.Id);
        }
    }
}