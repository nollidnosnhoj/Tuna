using Ardalis.Specification;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Features.Followers.CheckIfFollowing
{
    public sealed class CheckIfUserIsFollowingSpecification : Specification<FollowedUser>
    {
        public CheckIfUserIsFollowingSpecification(string targetUsername, string observerId)
        {
            Query.AsNoTracking()
                .Include(u => u.Target)
                .Where(u => u.ObserverId == observerId && u.Target.UserName == targetUsername.Trim().ToLower());
        }
    }
}