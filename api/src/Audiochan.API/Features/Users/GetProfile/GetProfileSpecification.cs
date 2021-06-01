using Ardalis.Specification;
using Audiochan.API.Mappings;
using Audiochan.Core.Entities;

namespace Audiochan.API.Features.Users.GetProfile
{
    public sealed class GetProfileSpecification : Specification<User, ProfileViewModel>
    {
        public GetProfileSpecification(string username, string currentUserId = "")
        {
            Query.AsNoTracking()
                .Include(u => u.FollowersTable)
                .Include(u => u.FollowingsTable)
                .Include(u => u.Audios)
                .Where(u => u.UserName == username.Trim().ToLower());

            Query.Select<User, ProfileViewModel>(UserMappings.ProfileProjection(currentUserId));
        }
    }
}