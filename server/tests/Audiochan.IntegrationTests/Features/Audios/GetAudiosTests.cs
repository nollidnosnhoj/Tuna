using System.Threading.Tasks;
using Audiochan.Core.Common.Builders;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.UnitTests;
using FluentAssertions;
using Xunit;

namespace Audiochan.IntegrationTests.Features.Audios
{
    [Collection(nameof(SliceFixture))]
    public class GetAudiosTests
    {
        private readonly SliceFixture _fixture;

        public GetAudiosTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ShouldNotGetUnlistedOrPrivateAudios()
        {
            var (ownerId, _) = await _fixture.RunAsAdministratorAsync();
            
            for (var i = 0; i < 10; i++)
            {
                var audioBuilder = new AudioBuilder()
                    .UseTestDefaults(ownerId)
                    .SetVisibility(Visibility.Public);

                switch (i)
                {
                    case 6:
                    case 7:
                        audioBuilder = audioBuilder.SetVisibility(Visibility.Unlisted);
                        break;
                    case 8:
                    case 9:
                        audioBuilder = audioBuilder.SetVisibility(Visibility.Private);
                        break;
                }

                var audio = await audioBuilder.BuildAsync();

                await _fixture.InsertAsync(audio);
            }
            
            var result = await _fixture.SendAsync(new GetAudioListRequest());

            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            result.Items.Count.Should().BeGreaterThan(0);
            result.Items.Count.Should().Be(6);
        }
    }
}