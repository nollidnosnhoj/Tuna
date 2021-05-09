using System;
using System.Threading.Tasks;
using Audiochan.Core.Common.Builders;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Infrastructure.Shared;
using Audiochan.UnitTests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Xunit;

namespace Audiochan.IntegrationTests.Services
{
    [Collection(nameof(SliceFixture))]
    public class CleanupServiceTests
    {
        private readonly SliceFixture _sliceFixture;

        public CleanupServiceTests(SliceFixture sliceFixture)
        {
            _sliceFixture = sliceFixture;
        }

        [Fact]
        public async Task CleanUnpublishAudiosTest()
        {
            // Assign
            var (ownerId, _) = await _sliceFixture.RunAsAdministratorAsync();
            var dateTimeProvider = await _sliceFixture.ExecuteScopeAsync(sp => 
                Task.FromResult(sp.GetRequiredService<IDateTimeProvider>()));
            var audio1 = await new AudioBuilder()
                .UseTestDefaults(ownerId)
                .SetCreatedDate(dateTimeProvider.Now.Minus(Duration.FromDays(2)))
                .BuildAsync();
            
            var audio2 = await new AudioBuilder()
                .UseTestDefaults(ownerId)
                .SetCreatedDate(dateTimeProvider.Now.Minus(Duration.FromDays(2)))
                .BuildAsync();
            
            var audio3 = await new AudioBuilder()
                .UseTestDefaults(ownerId)
                .SetCreatedDate(dateTimeProvider.Now.Minus(Duration.FromDays(2)))
                .BuildAsync();

            var audio4 = await new AudioBuilder()
                .UseTestDefaults(ownerId)
                .BuildAsync();

            await _sliceFixture.InsertAsync(audio1, audio2, audio3, audio4);

            var rowAffected = await _sliceFixture.ExecuteScopeAsync(sp =>
            {
                var cleanupService = sp.GetRequiredService<CleanupService>();
                return Task.FromResult(cleanupService.CleanUnpublishedAudios());
            });

            // ACT
            rowAffected.Should().Be(3);
        }
    }
}