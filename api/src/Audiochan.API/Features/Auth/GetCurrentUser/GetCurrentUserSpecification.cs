using Ardalis.Specification;
using Audiochan.API.Mappings;
using Audiochan.Core.Entities;

namespace Audiochan.API.Features.Auth.GetCurrentUser
{
    public sealed class GetCurrentUserSpecification : Specification<User, CurrentUserViewModel>
    {
        public GetCurrentUserSpecification(string currentUserId)
        {
            Query.AsNoTracking()
                .Where(u => u.Id == currentUserId);

            Query.Select<User, CurrentUserViewModel>(UserMappings.CurrentUserProjection());
        }
    }
}