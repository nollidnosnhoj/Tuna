using Ardalis.Specification;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Features.Auth.GetCurrentUser
{
    public sealed class GetCurrentUserSpecification : Specification<User, CurrentUserViewModel>
    {
        public GetCurrentUserSpecification(string currentUserId)
        {
            Query.Select(UserMappingExtensions.CurrentUserProjection())
                .AsNoTracking()
                .Where(u => u.Id == currentUserId);
        }
    }
}