using System.Linq;
using Ardalis.Specification;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Users
{
    public sealed class LoadAudioWithFavoritedUsersSpecification : Specification<Audio>
    {
        public LoadAudioWithFavoritedUsersSpecification(long audioId, long observerId)
        {
            if (observerId > 0)
            {
                Query.Include(a =>
                    a.FavoriteAudios.Where(fa => fa.UserId == observerId));
            }
            else
            {
                Query.Include(a => a.FavoriteAudios);
            }

            Query.Where(a => a.Id == audioId);
        }
    }
}