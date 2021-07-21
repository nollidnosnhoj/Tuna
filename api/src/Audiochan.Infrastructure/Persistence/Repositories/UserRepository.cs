using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Features.Auth.GetCurrentUser;
using Audiochan.Core.Features.Followers.GetFollowers;
using Audiochan.Core.Features.Followers.GetFollowings;
using Audiochan.Core.Features.Users.GetProfile;
using Audiochan.Core.Features.Users.GetUserAudios;
using Audiochan.Core.Features.Users.GetUserFavoriteAudios;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using Audiochan.Infrastructure.Persistence.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class UserRepository : EfRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext, ICurrentUserService currentUserService, IMapper mapper) 
            : base(dbContext, currentUserService, mapper)
        {
        }

        public async Task<CurrentUserViewModel?> GetAuthenticated(CancellationToken cancellationToken = default)
        {
            var currentUserId = CurrentUserService.GetUserId();
            return await DbSet
                .AsNoTracking()
                .Where(u => u.Id == currentUserId)
                .ProjectTo<CurrentUserViewModel>(Mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> LoadWithRefreshTokens(string refreshToken, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens
                    .Any(t => t.Token == refreshToken && t.UserId == u.Id), cancellationToken);
        }

        public async Task<ProfileViewModel?> GetProfile(string username, CancellationToken cancellationToken = default)
        {
            return await DbSet.AsNoTracking()
                .Include(u => u.Followers)
                .Include(u => u.Followings)
                .Include(u => u.Audios)
                .Where(u => u.UserName == username.Trim().ToLower())
                .ProjectTo<ProfileViewModel>(Mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> LoadWithFollowers(string targetId, string observerId = "", CancellationToken cancellationToken = default)
        {
            var queryable = DbSet
                .IgnoreQueryFilters()
                .Where(u => u.Id == targetId);

            queryable = string.IsNullOrEmpty(observerId)
                ? queryable.Include(u => u.Followers)
                : queryable.Include(u => u.Followers.Where(f => f.ObserverId == observerId));

            return await queryable.SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<PagedListDto<AudioViewModel>> GetUserAudios(GetUsersAudioQuery query, CancellationToken cancellationToken = default)
        {
            var currentUserId = CurrentUserService.GetUserId();

            var queryable = DbSet
                .AsNoTracking()
                .Include(u => u.Audios)
                .Where(u => query.Username == u.UserName.ToLower())
                .SelectMany(a => a.Audios);

            queryable = !string.IsNullOrEmpty(currentUserId) 
                ? queryable.Where(a => a.Visibility == Visibility.Public || a.UserId == currentUserId) 
                : queryable.Where(a => a.Visibility == Visibility.Public);

            return await queryable
                .ProjectTo<AudioViewModel>(Mapper.ConfigurationProvider)
                .PaginateAsync(query, cancellationToken);
        }

        public async Task<PagedListDto<AudioViewModel>> GetUserFavoriteAudios(GetUserFavoriteAudiosQuery query, 
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Include(u => u.FavoriteAudios)
                .ThenInclude(fa => fa.Audio)
                .Where(u => u.UserName == query.Username)
                .SelectMany(u => u.FavoriteAudios)
                .OrderByDescending(fa => fa.FavoriteDate)
                .Select(fa => fa.Audio)
                .ProjectTo<AudioViewModel>(Mapper.ConfigurationProvider)
                .PaginateAsync(query, cancellationToken);
        }

        public async Task<PagedListDto<FollowerViewModel>> GetFollowers(GetUserFollowersQuery query, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Include(u => u.Followers)
                .Where(u => u.UserName == query.Username)
                .SelectMany(u => u.Followers)
                .OrderByDescending(fu => fu.FollowedDate)
                .ProjectTo<FollowerViewModel>(Mapper.ConfigurationProvider)
                .PaginateAsync(query, cancellationToken);
        }

        public async Task<PagedListDto<FollowingViewModel>> GetFollowings(GetUserFollowingsQuery query, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Include(u => u.Followings)
                .Where(u => u.UserName == query.Username)
                .SelectMany(u => u.Followings)
                .OrderByDescending(fu => fu.FollowedDate)
                .ProjectTo<FollowingViewModel>(Mapper.ConfigurationProvider)
                .PaginateAsync(query, cancellationToken);
        }

        public async Task<List<string>> GetFollowingIds(string userId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Include(u => u.Followings)
                .AsNoTracking()
                .Where(user => user.Id == userId)
                .SelectMany(u => u.Followings.Select(f => f.TargetId))
                .ToListAsync(cancellationToken);
        }
    }
}