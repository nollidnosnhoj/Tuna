using Ardalis.Specification;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Users
{
    public sealed class GetFollowerByTargetNameSpecification : Specification<FollowedUser>
    {
        public GetFollowerByTargetNameSpecification(string username)
        {
            Query.AsNoTracking();
            Query.Where(u => u.Target.UserName == username);
            Query.OrderByDescending(u => u.FollowedDate);
        }
    }
}