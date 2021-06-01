using Ardalis.Specification;
using Audiochan.Core.Entities;

namespace Audiochan.API.Features.Followers.SetFollow
{
    public sealed class GetTargetUserSpecification : Specification<User>
    {
        public GetTargetUserSpecification(string targetUsername)
        {
            Query.Include(u => u.FollowersTable)
                .Where(u => u.UserName == targetUsername.Trim().ToLower());
        }
    }
}