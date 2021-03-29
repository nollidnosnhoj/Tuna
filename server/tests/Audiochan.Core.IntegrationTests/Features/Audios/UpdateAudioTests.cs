﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Audios.UpdateAudio;
using Audiochan.Core.UnitTests.Builders;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    [Collection(nameof(SliceFixture))]
    public class UpdateAudioTests
    {
        private readonly SliceFixture _fixture;

        public UpdateAudioTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ShouldNotUpdate_WhenUserCannotModify()
        {
            // Assign
            var (ownerId, _) = await _fixture
                .RunAsUserAsync("kopacetic", Guid.NewGuid().ToString(), Array.Empty<string>());

            var audio = new AudioBuilder(ownerId, "testaudio.mp3").Build();

            await _fixture.InsertAsync(audio);

            // Act
            await _fixture.RunAsDefaultUserAsync();

            var command = new UpdateAudioRequest
            {
                Id = audio.Id,
                Title = "New Audio Title PogChamp"
            };

            var result = await _fixture.SendAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().Be(false);
            result.ErrorCode.Should().Be(ResultError.Forbidden);
        }

        [Fact]
        public async Task ShouldUpdate_WhenInputIsValid()
        {
            // Assign
            var (ownerId, _) = await _fixture
                .RunAsUserAsync("kopacetic", Guid.NewGuid().ToString(), Array.Empty<string>());

            var audio = new AudioBuilder(ownerId, "testaudio.mp3")
                .Build();

            await _fixture.InsertAsync(audio);
            
            // Act
            var command = new UpdateAudioRequest
            {
                Id = audio.Id,
                Title = "This is a new Title",
                Description = "This is a test description",
                Tags = new List<string> {"apples", "oranges", "caramel"},
            };

            var result = await _fixture.SendAsync(command);

            var created = await _fixture.ExecuteDbContextAsync(database =>
            {
                return database.Audios
                    .Include(a => a.Tags)
                    .Include(a => a.User)
                    .SingleOrDefaultAsync(a => a.Id == audio.Id);
            });

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().Be(true);
            result.Data.Should().NotBeNull();
            result.Data.Should().BeOfType<AudioDetailViewModel>();
            result.Data.Should().NotBeNull();
            result.Data.Title.Should().Be(command.Title);
            result.Data.Description.Should().Be(command.Description);
            result.Data.Tags.Length.Should().Be(3);
            result.Data.Tags.Should().Contain(x => x == "apples");
            result.Data.Tags.Should().Contain(x => x == "oranges");
            result.Data.Tags.Should().Contain(x => x == "caramel");

            created.Should().NotBeNull();
            created.Title.Should().Be(command.Title);
            created.Description.Should().Be(command.Description);
            created.Tags.Count.Should().Be(3);
            created.Tags.Should().Contain(x => x.Name == "apples");
            created.Tags.Should().Contain(x => x.Name == "oranges");
            created.Tags.Should().Contain(x => x.Name == "caramel");
        }
    }
}