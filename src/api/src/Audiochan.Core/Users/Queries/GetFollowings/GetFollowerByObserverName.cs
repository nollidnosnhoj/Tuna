using Ardalis.Specification;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Users.Queries
{
    public sealed class GetFollowerByObserverName : Specification<FollowedUser>
    {
        public GetFollowerByObserverName(string username)
        {
            Query.AsNoTracking();
            Query.Where(u => u.Observer.UserName == username);
            Query.OrderByDescending(u => u.FollowedDate);
        }
    }
}