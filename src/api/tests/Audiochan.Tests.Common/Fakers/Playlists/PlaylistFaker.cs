using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Bogus;
using Slugify;

namespace Audiochan.Tests.Common.Fakers.Playlists
{
    public sealed class PlaylistFaker : Faker<Playlist>
    {
        public PlaylistFaker(long userId)
        {
            RuleFor(x => x.UserId, userId);
            RuleFor(x => x.Title, f => f.Random.String2(3, 30));
            RuleFor(x => x.Description, f => f.Lorem.Sentences(2));
            RuleFor(x => x.Visibility, f => f.PickRandom<Visibility>());
            RuleFor(x => x.Slug, (_, p) => new SlugHelper().GenerateSlug(p.Title));
            RuleFor(x => x.Secret,
                (faker, audio) => audio.Visibility == Visibility.Private ? faker.Random.String2(10) : null);
        }
    }
}