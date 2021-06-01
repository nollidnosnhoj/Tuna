using System;
using Ardalis.Specification;
using Audiochan.Core.Entities;

namespace Audiochan.API.Features.FavoriteAudios.SetFavorite
{
    public sealed class GetTargetAudioForFavoritingSpecification : Specification<Audio>
    {
        public GetTargetAudioForFavoritingSpecification(Guid audioId)
        {
            Query.Include(a => a.Favorited)
                .Where(a => a.Id == audioId);
        }
    }
}