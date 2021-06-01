using System.Linq;
using Ardalis.Specification;
using Audiochan.Core.Entities;

namespace Audiochan.API.Features.Followers.CheckIfFollowing
{
    public sealed class CheckIfUserIsFollowingSpecification : Specification<User>
    {
        public CheckIfUserIsFollowingSpecification(string targetUsername, string observerId)
        {
            Query.AsNoTracking()
                .Include(u => u.Followers)
                .Where(u => u.Followers.Any(x => x.Id == observerId) 
                            && u.UserName == targetUsername.Trim().ToLower());
        }
    }
}