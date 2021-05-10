using System;
using System.IO;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Infrastructure.Shared;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
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
        public async Task CleanupUnPublishedAudiosSuccessfully()
        {
            // Assign
            var (ownerId, _) = await _sliceFixture.RunAsAdministratorAsync();
            var nowDate = _sliceFixture.SetCurrentTime(new DateTime(2021, 5, 5));
            var unPublishCreatedDate = nowDate.AddDays(-2);

            var unPublishedAudioFaker = new Faker<Audio>()
                .RuleFor(x => x.Id, _ => Nanoid.Nanoid.Generate())
                .RuleFor(x => x.FileExt, f => f.System.FileExt("audio/mpeg"))
                .RuleFor(x => x.OriginalFileName, f => f.System.FileName(".mp3"))
                .RuleFor(x => x.FileSize, f => f.Random.Number(1, 2_000_000))
                .RuleFor(x => x.Duration, f => f.Random.Number(1, 300))
                .FinishWith((_, a) =>
                {
                    a.Title = Path.GetFileNameWithoutExtension(a.OriginalFileName);
                    a.Created = unPublishCreatedDate;
                    a.UserId = ownerId;
                    a.FileName = a.Id + a.FileExt;
                });

            var unPublishedAudios = unPublishedAudioFaker.Generate(3);
            
            var publishedAudioFaker = new Faker<Audio>()
                .RuleFor(x => x.Id, _ => Nanoid.Nanoid.Generate())
                .RuleFor(x => x.FileExt, f => f.System.FileExt("audio/mpeg"))
                .RuleFor(x => x.OriginalFileName, f => f.System.FileName(".mp3"))
                .RuleFor(x => x.FileSize, f => f.Random.Number(1, 2_000_000))
                .RuleFor(x => x.Duration, f => f.Random.Number(1, 300))
                .FinishWith((_, a) =>
                {
                    a.Title = Path.GetFileNameWithoutExtension(a.OriginalFileName);
                    a.UserId = ownerId;
                    a.FileName = a.Id + a.FileExt;
                });

            var publishedAudios = publishedAudioFaker.Generate(3);

            await _sliceFixture.ExecuteDbContextAsync(db =>
            {
                db.Audios.AddRangeAsync(unPublishedAudios);
                db.Audios.AddRangeAsync(publishedAudios);
                return db.SaveChangesAsync();
            });

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