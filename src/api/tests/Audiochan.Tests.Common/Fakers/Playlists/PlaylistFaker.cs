using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Bogus;

namespace Audiochan.Tests.Common.Fakers.Playlists
{
    public sealed class PlaylistFaker : Faker<Playlist>
    {
        public PlaylistFaker(long userId, bool shouldGenerateId = false)
        {
            if (shouldGenerateId)
                RuleFor(x => x.Id, f => f.Random.Guid());
            RuleFor(x => x.UserId, userId);
            RuleFor(x => x.Title, f => f.Random.String2(3, 30));
            RuleFor(x => x.Description, f => f.Lorem.Sentences(2));
            RuleFor(x => x.Visibility, f => f.PickRandom<Visibility>());
        }
    }
}