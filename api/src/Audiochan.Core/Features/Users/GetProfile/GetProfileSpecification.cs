using Ardalis.Specification;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Features.Users.GetProfile
{
    public sealed class GetProfileSpecification : Specification<User, ProfileViewModel>
    {
        public GetProfileSpecification(string username, string currentUserId = "")
        {
            Query.AsNoTracking()
                .Include(u => u.Followers)
                .Include(u => u.Followings)
                .Include(u => u.Audios)
                .Where(u => u.UserName == username.Trim().ToLower());

            Query.Select(UserMappingExtensions.ProfileProjection(currentUserId));
        }
    }
}