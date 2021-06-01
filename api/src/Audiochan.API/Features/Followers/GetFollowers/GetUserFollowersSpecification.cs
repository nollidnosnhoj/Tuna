using Ardalis.Specification;
using Audiochan.API.Mappings;
using Audiochan.Core.Entities;

namespace Audiochan.API.Features.Followers.GetFollowers
{
    public sealed class GetUserFollowersSpecification : Specification<FollowedUser, FollowerViewModel>
    {
        public GetUserFollowersSpecification(string username)
        {
            Query.AsNoTracking()
                .Include(u => u.Target)
                .Include(u => u.Observer)
                .Where(u => u.Target.UserName == username.Trim().ToLower())
                .OrderByDescending(x => x.FollowedDate);

            Query.Select(FollowedUserMappings.FollowerToListProjection());
        }
    }
}