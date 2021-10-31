using Ardalis.Specification;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Auth.Queries
{
    public sealed class GetCurrentUserSpecification : Specification<User>
    {
        public GetCurrentUserSpecification(long userId)
        {
            Query.AsNoTracking();
            Query.Where(u => u.Id == userId);
        }
    }
}