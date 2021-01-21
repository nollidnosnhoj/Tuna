using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Followers.Models;

namespace Audiochan.Core.Interfaces
{
    public interface IFollowerService
    {
        Task<List<FollowUserViewModel>> GetUsersFollowers(string username, PaginationQuery paginationQuery,
            CancellationToken cancellationToken = default);

        Task<List<FollowUserViewModel>> GetUsersFollowings(string username, PaginationQuery paginationQuery,
            CancellationToken cancellationToken = default);

        Task<bool> CheckFollowing(long userId, string username, CancellationToken cancellationToken = default);

        Task<IResult> Follow(long userId, string username, CancellationToken cancellationToken = default);
        
        Task<IResult> Unfollow(long userId, string username, CancellationToken cancellationToken = default);
    }
}