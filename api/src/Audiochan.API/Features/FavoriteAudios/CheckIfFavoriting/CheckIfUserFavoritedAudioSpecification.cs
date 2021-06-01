using System;
using Ardalis.Specification;
using Audiochan.Core.Entities;

namespace Audiochan.API.Features.FavoriteAudios.CheckIfFavoriting
{
    public sealed class CheckIfUserFavoritedAudioSpecification : Specification<FavoriteAudio>
    {
        public CheckIfUserFavoritedAudioSpecification(Guid audioId, string userId)
        {
            Query.AsNoTracking()
                .Where(u => u.AudioId == audioId && u.UserId == userId);
        }
    }
}