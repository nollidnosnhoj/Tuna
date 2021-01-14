using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Users.Models;
using Audiochan.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public UserService(UserManager<User> userManager, ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
        }

        public async Task<IResult<CurrentUserViewModel>> GetCurrentUser(long authUserId, 
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.Users
                .AsNoTracking()
                .Where(u => u.Id == authUserId)
                .SingleOrDefaultAsync(cancellationToken);

            if (user == null)
                return Result<CurrentUserViewModel>.Fail(ResultErrorCode.Unauthorized);

            var roles = await _userManager.GetRolesAsync(user);

            return Result<CurrentUserViewModel>.Success(new CurrentUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.UserName,
                Roles = roles
            });
        }

        public async Task<IResult<UserDetailsViewModel>> GetUserDetails(string username, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetUserId();
            
            var profile = await _userManager.Users
                .AsNoTracking()
                .Include(u => u.Followers)
                .Include(u => u.Followings)
                .Include(u => u.Audios)
                .Where(u => u.UserName == username)
                .Select(MapProjections.UserDetails(currentUserId))
                .SingleOrDefaultAsync(cancellationToken);

            return profile == null
                ? Result<UserDetailsViewModel>.Fail(ResultErrorCode.NotFound)
                : Result<UserDetailsViewModel>.Success(profile);
        }

        public async Task<IResult> UpdateUsername(long userId, string newUsername, 
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId + "");
            if (user == null) return Result.Fail(ResultErrorCode.Unauthorized);
            var result = await _userManager.SetUserNameAsync(user, newUsername);
            if (!result.Succeeded) result.ToResult();
            await _userManager.UpdateNormalizedUserNameAsync(user);
            return Result.Success();
        }

        public async Task<IResult> UpdateEmail(long userId, string newEmail, 
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId + "");
            if (user == null) return Result.Fail(ResultErrorCode.Unauthorized);
            
            // TEMPORARY UNTIL EMAIL CONFIRMATION IS SETUP
            var result = await _userManager.SetEmailAsync(user, newEmail);
            if (!result.Succeeded) return result.ToResult();
            await _userManager.UpdateNormalizedEmailAsync(user);
            return Result.Success();
        }

        public async Task<IResult> UpdatePassword(long userId, ChangePasswordRequest request, 
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId + "");
            if (user == null) return Result.Fail(ResultErrorCode.Unauthorized);
            // TEMPORARY UNTIL EMAIL CONFIRMATION IS SETUP
            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            return result.ToResult();
        }

        public async Task<IResult> UpdateUser(long userId, UpdateUserDetailsRequest request, 
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId + "");
            if (user == null) return Result.Fail(ResultErrorCode.Unauthorized);
            user.About = request.About ?? user.About;
            user.Website = request.Website ?? user.Website;
            await _userManager.UpdateAsync(user);
            return Result.Success();
        }

        public async Task<bool> CheckIfUsernameExists(
            string username
            , CancellationToken cancellationToken = default)
        {
            return await _userManager.Users
                .AsNoTracking()
                .AnyAsync(u => u.UserName == username, cancellationToken);
        }
        
        public async Task<bool> CheckIfEmailExists(
            string email
            , CancellationToken cancellationToken = default)
        {
            return await _userManager.Users
                .AsNoTracking()
                .AnyAsync(u => u.Email == email, cancellationToken);
        }
    }
}