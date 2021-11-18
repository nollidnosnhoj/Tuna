using System.Linq;
using Ardalis.Specification;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Users.Commands
{
    public sealed class LoadArtistWithFollowersSpecification : Specification<Artist>
    {
        public LoadArtistWithFollowersSpecification(long targetId, long observerId)
        {
            if (observerId > 0)
            {
                Query.Include(a =>
                    a.Followers.Where(fa => fa.ObserverId == observerId));
            }
            else
            {
                Query.Include(a => a.Followers);
            }

            Query.Where(a => a.Id == targetId);
        }
    }
}