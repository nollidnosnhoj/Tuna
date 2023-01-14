using System.Linq;
using Audiochan.Core.Features.Users.Queries.GetFollowers;
using Audiochan.Core.Features.Users.Queries.GetFollowings;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Features.Users.Mappings;

public static partial class DtoMappings
{
    public static IQueryable<FollowerViewModel> ProjectToFollower(this IQueryable<FollowedUser> queryable)
    {
        return queryable.Select(x => new FollowerViewModel
        {
            UserName = x.Observer.UserName,
            Picture = x.Observer.Picture != null ? MediaLinkConstants.USER_PICTURE + x.Observer.Picture : null,
            FollowedDate = x.FollowedDate
        });
    }
    
    public static IQueryable<FollowingViewModel> ProjectToFollowing(this IQueryable<FollowedUser> queryable)
    {
        return queryable.Select(x => new FollowingViewModel
        {
            UserName = x.Target.UserName,
            Picture = x.Target.Picture != null ? MediaLinkConstants.USER_PICTURE + x.Target.Picture : null,
            FollowedDate = x.FollowedDate
        });
    }
}