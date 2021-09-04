using System.Collections.Generic;
using Audiochan.Core.Playlists.CreatePlaylist;
using Bogus;

namespace Audiochan.Tests.Common.Fakers.Playlists
{
    public sealed class CreatePlaylistCommandFaker : Faker<CreatePlaylistCommand>
    {
        public CreatePlaylistCommandFaker()
        {
            RuleFor(x => x.Title, f => f.Random.String2(15));
        }
        
        public CreatePlaylistCommandFaker(ICollection<long> audioIds)
        {
            RuleFor(x => x.AudioIds, () => audioIds);
        }
    }
}