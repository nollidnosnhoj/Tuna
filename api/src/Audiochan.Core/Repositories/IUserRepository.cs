using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Audios.GetLatestAudios;
using Audiochan.Core.Features.Auth.GetCurrentUser;
using Audiochan.Core.Features.Followers.GetFollowers;
using Audiochan.Core.Features.Followers.GetFollowings;
using Audiochan.Core.Features.Users.GetProfile;
using Audiochan.Core.Features.Users.GetUserAudios;
using Audiochan.Core.Features.Users.GetUserFavoriteAudios;

namespace Audiochan.Core.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<CurrentUserViewModel?> GetAuthenticated(CancellationToken cancellationToken = default);
        Task<User?> LoadWithRefreshTokens(string refreshToken, CancellationToken cancellationToken = default);
        Task<ProfileViewModel?> GetProfile(string username, CancellationToken cancellationToken = default);
        Task<User?> LoadWithFollowers(string targetId, string observerId = "", CancellationToken cancellationToken = default);

        Task<PagedListDto<AudioViewModel>> GetUserAudios(GetUsersAudioQuery query,
            CancellationToken cancellationToken = default);

        Task<PagedListDto<AudioViewModel>> GetUserFavoriteAudios(GetUserFavoriteAudiosQuery query,
            CancellationToken cancellationToken = default);

        Task<PagedListDto<FollowerViewModel>> GetFollowers(GetUserFollowersQuery query,
            CancellationToken cancellationToken = default);

        Task<PagedListDto<FollowingViewModel>> GetFollowings(GetUserFollowingsQuery query,
            CancellationToken cancellationToken = default);

        Task<List<string>> GetFollowingIds(string userId, CancellationToken cancellationToken = default);
    }
}