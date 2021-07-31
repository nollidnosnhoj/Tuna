﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    public class UpdateAudioTests : TestBase
    {
        public UpdateAudioTests(TestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task ShouldNotUpdate_WhenUserCannotModify()
        {
            // Assign
            var (ownerId, _) = await RunAsUserAsync("kopacetic", 
                Guid.NewGuid().ToString(), 
                Array.Empty<string>());

            var audio = new AudioFaker(ownerId).Generate();

            Insert(audio);

            // Act
            await RunAsDefaultUserAsync();

            var command = new UpdateAudioRequestFaker(audio.Id).Generate();

            var result = await SendAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().Be(false);
            result.ErrorCode.Should().Be(ResultError.Forbidden);
        }

        [Fact]
        public async Task ShouldUpdateSuccessfully()
        {
            // Assign
            var (ownerId, _) = await RunAsUserAsync("kopacetic", 
                Guid.NewGuid().ToString(),
                Array.Empty<string>());

            var audio = new AudioFaker(ownerId).Generate();

            Insert(audio);
            
            // Act
            var command = new UpdateAudioRequestFaker(audio.Id).Generate();

            await SendAsync(command);

            var created = ExecuteDbContext(database =>
            {
                return database.Audios
                    .Include(a => a.Tags)
                    .Include(a => a.User)
                    .SingleOrDefault(a => a.Id == audio.Id);
            });

            // Assert
            created.Should().NotBeNull();
            created!.Title.Should().Be(command.Title);
            created.Description.Should().Be(command.Description);
            created.Tags.Count.Should().Be(command.Tags!.Count);
        }

        [Fact]
        public async Task ShouldInvalidateCacheSuccessfully()
        {
            // Assign
            var (userId, _) = await RunAsDefaultUserAsync();
            var audio = new AudioFaker(userId).Generate();
            Insert(audio);
            await SendAsync(new GetAudioQuery(audio.Id));
            var command = new UpdateAudioRequestFaker(audio.Id).Generate();
            await SendAsync(command);
            
            // Act
            var (cacheExists, _) = await GetCache<AudioViewModel>(CacheKeys.Audio.GetAudio(audio.Id));
            
            // Assert
            cacheExists.Should().BeFalse();
        }
    }
}