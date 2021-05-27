using Ardalis.Specification;
using Audiochan.Core.Entities;

namespace Audiochan.API.Features.Audios.GetAudioFeed
{
    public sealed class GetFollowingIdsSpecification : Specification<FollowedUser, string>
    {
        public GetFollowingIdsSpecification(string userId)
        {
            Query.AsNoTracking()
                .Where(user => user.ObserverId == userId);

            Query.Select(user => user.TargetId);
        }
    }
}