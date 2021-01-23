using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Followers.Models;

namespace Audiochan.Core.Interfaces
{
    public interface IFollowerService
    {
        Task<PagedList<FollowUserViewModel>> GetUsersFollowers(string username, PaginationQuery paginationQuery,
            CancellationToken cancellationToken = default);

        Task<PagedList<FollowUserViewModel>> GetUsersFollowings(string username, PaginationQuery paginationQuery,
            CancellationToken cancellationToken = default);

        Task<bool> CheckFollowing(string userId, string username, CancellationToken cancellationToken = default);

        Task<IResult> Follow(string userId, string username, CancellationToken cancellationToken = default);
        
        Task<IResult> Unfollow(string userId, string username, CancellationToken cancellationToken = default);
    }
}