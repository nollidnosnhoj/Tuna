using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios.SearchAudios;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    [Collection(nameof(SliceFixture))]
    public class SearchAudioTests
    {
        private readonly SliceFixture _fixture;

        public SearchAudioTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task SearchForAudiosSuccessfully()
        {
            var faker = new Faker();

            // create tags
            var tags = Enumerable.Range(1, 5)
                .Select(_ => faker.Random.Word().GenerateSlug())
                .ToList();


            // create posts
            var (ownerId, _) = await _fixture.RunAsAdministratorAsync();

            var audioFaker = new Faker<Audio>()
                .RuleFor(x => x.Size, f => f.Random.Number(1, 2_000_000))
                .RuleFor(x => x.Duration, f => f.Random.Number(1, 300))
                .RuleFor(x => x.Visibility, _ => Visibility.Public)
                .FinishWith((f, a) =>
                {
                    a.Tags = new List<Tag> {new() {Name = tags[0]}, new() {Name = tags[1]}};
                    a.Title = Path.GetFileNameWithoutExtension(f.System.FileName(".mp3"));
                    a.UserId = ownerId;
                    a.File = "test.mp3";
                });

            var audio = audioFaker.Generate();

            await _fixture.InsertAsync(audio);

            // Act
            var request = new SearchAudiosQuery
            {
                Q = audio.Title,
                Tags = $"{tags[0]},{tags[1]}"
            };

            var results = await _fixture.SendAsync(request);

            //Assert 
            results.Should().NotBeNull();
            results.Count.Should().BeGreaterThan(0);
            results.Items.Count.Should().BeGreaterThan(0);
            results.Items.Should().Contain(x => x.Id == audio.Id);
        }
    }
}