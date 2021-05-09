using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Audiochan.Core.Common.Builders;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.PublishAudio;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.UnitTests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Xunit;

namespace Audiochan.IntegrationTests.Features.Audios
{
    [Collection(nameof(SliceFixture))]
    public class GetAudioTests
    {
        private readonly SliceFixture _fixture;

        public GetAudioTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ShouldNotGetAudio_WhenAudioIdIsInvalid()
        {
            // Assign
            var (ownerId, _) = await _fixture.RunAsDefaultUserAsync();
            var audio = await new AudioBuilder()
                .UseTestDefaults(ownerId, "myaudio.mp3")
                .BuildAsync();
            await _fixture.InsertAsync(audio);

            // Act
            var result = await _fixture.SendAsync(new GetAudioRequest(string.Empty));

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ShouldGetAudio()
        {
            // Assign
            var dateTimeProvider = await _fixture.ExecuteScopeAsync(sp => 
                Task.FromResult(sp.GetRequiredService<IDateTimeProvider>()));

            var (userId, _) = await _fixture.RunAsDefaultUserAsync();

            var userTags = new List<string>
            {
                "oranges", "apples"
            };

            var tags = await _fixture.ExecuteScopeWithTransactionAsync(sp =>
            {
                var repo = sp.GetRequiredService<ITagRepository>();
                return repo.GetListAsync(userTags);
            });

            var audio = await new AudioBuilder()
                .AddFileName("test.mp3")
                .AddTitle("Test Song")
                .AddFileSize(100)
                .AddDuration(100)
                .AddTags(tags)
                .AddUserId(userId)
                .SetPublic(true)
                .SetPublishToTrue(dateTimeProvider.Now)
                .BuildAsync();
            
            await _fixture.InsertAsync(audio);

            // Act
            var result = await _fixture.SendAsync(new GetAudioRequest(audio.Id));

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<AudioDetailViewModel>();
            result.Title.Should().Be(audio.Title);
            result.Tags.Length.Should().Be(2);
            result.Tags.Should().Contain("apples");
            result.Tags.Should().Contain("oranges");
        }
    }
}