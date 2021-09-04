using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Features.Audios.SearchAudios;
using Audiochan.Domain.Entities;
using Audiochan.Tests.Common.Fakers.Audios;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    public class SearchAudioTests : TestBase
    {
        public SearchAudioTests(TestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task SearchForAudiosSuccessfully()
        {
            var faker = new Faker();

            // create tags
            var tags = Enumerable.Range(1, 5)
                .Select(_ => faker.Random.String2(5, 10))
                .ToList();


            // create posts
            var (userId, _) = await RunAsDefaultUserAsync();

            var audioFaker = new AudioFaker(userId)
                .FinishWith((f, a) =>
                {
                    a.Tags = new List<Tag> {new() {Name = tags[0]}, new() {Name = tags[1]}};
                });

            var audio = audioFaker.Generate();

            InsertIntoDatabase(audio);

            // Act
            var request = new SearchAudiosQuery
            {
                Q = audio.Title,
                Tags = $"{tags[0]},{tags[1]}"
            };

            var results = await SendAsync(request);

            //Assert 
            results.Should().NotBeNull();
            results.Items.Count.Should().BeGreaterThan(0);
            results.Items.Should().Contain(x => x.Id == audio.Id);
        }
    }
}