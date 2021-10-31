using Ardalis.Specification;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Users
{
    public sealed class GetUserAudiosSpecification : Specification<Audio>
    {
        public GetUserAudiosSpecification(string username)
        {
            Query.AsNoTracking();
            Query.Where(a => a.User.UserName == username);
            Query.OrderByDescending(a => a.Id);
        }
    }
}