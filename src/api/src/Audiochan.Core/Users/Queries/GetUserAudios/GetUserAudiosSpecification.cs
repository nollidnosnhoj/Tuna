using Ardalis.Specification;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Users.Queries
{
    public sealed class GetUserAudiosSpecification : Specification<Audio>
    {
        public GetUserAudiosSpecification(string username)
        {
            Query.AsNoTracking();
            Query.Where(a => a.Artist.UserName == username);
            Query.OrderByDescending(a => a.Id);
        }
    }
}