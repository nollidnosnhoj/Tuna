using Ardalis.Specification;
using Audiochan.API.Mappings;
using Audiochan.Core.Entities;

namespace Audiochan.API.Features.Followers.GetFollowings
{
    public sealed class GetUserFollowingsSpecification : Specification<FollowedUser, FollowingViewModel>
    {
        public GetUserFollowingsSpecification(string username)
        {
            Query.AsNoTracking()
                .Include(u => u.Target)
                .Include(u => u.Observer)
                .Where(u => u.Observer.UserName == username.Trim().ToLower())
                .OrderByDescending(x => x.FollowedDate);

            Query.Select<FollowedUser, FollowingViewModel>(FollowedUserMappings.FollowingToListProjection());
        }
    }
}