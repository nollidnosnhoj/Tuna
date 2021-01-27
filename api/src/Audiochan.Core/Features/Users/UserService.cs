using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Users.Mappings;
using Audiochan.Core.Features.Users.Models;
using Audiochan.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageService _imageService;

        public UserService(UserManager<User> userManager, ICurrentUserService currentUserService, IImageService imageService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
            _imageService = imageService;
        }

        public async Task<IResult<CurrentUserViewModel>> GetCurrentUser(string authUserId, 
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.Users
                .AsNoTracking()
                .Where(u => u.Id == authUserId)
                .SingleOrDefaultAsync(cancellationToken);

            if (user == null)
                return Result<CurrentUserViewModel>.Fail(ResultStatus.Unauthorized);

            var roles = await _userManager.GetRolesAsync(user);

            return Result<CurrentUserViewModel>.Success(new CurrentUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.UserName,
                Roles = roles
            });
        }

        public async Task<IResult<UserDetailsViewModel>> GetUserProfile(string username, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetUserId();
            
            var profile = await _userManager.Users
                .AsNoTracking()
                .Include(u => u.Followers)
                .Include(u => u.Followings)
                .Include(u => u.Audios)
                .Where(u => u.UserName == username)
                .Select(UserProfileMapping.Map(currentUserId))
                .SingleOrDefaultAsync(cancellationToken);

            return profile == null
                ? Result<UserDetailsViewModel>.Fail(ResultStatus.NotFound)
                : Result<UserDetailsViewModel>.Success(profile);
        }

        public async Task<IResult<string>> AddPicture(string userId, IFormFile file,
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return Result<string>.Fail(ResultStatus.Unauthorized);
            
            var blobDto = await _imageService.UploadUserImage(file, user.Id, cancellationToken);

            user.PictureUrl = blobDto.Url;
            await _userManager.UpdateAsync(user);
            return Result<string>.Success(blobDto.Url);
        }

        public async Task<IResult> UpdateUsername(string userId, string newUsername, 
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId + "");
            if (user == null) return Result.Fail(ResultStatus.Unauthorized);
            var result = await _userManager.SetUserNameAsync(user, newUsername);
            if (!result.Succeeded) result.ToResult();
            await _userManager.UpdateNormalizedUserNameAsync(user);
            return Result.Success();
        }

        public async Task<IResult> UpdateEmail(string userId, string newEmail, 
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId + "");
            if (user == null) return Result.Fail(ResultStatus.Unauthorized);
            
            // TEMPORARY UNTIL EMAIL CONFIRMATION IS SETUP
            var result = await _userManager.SetEmailAsync(user, newEmail);
            if (!result.Succeeded) return result.ToResult();
            await _userManager.UpdateNormalizedEmailAsync(user);
            return Result.Success();
        }

        public async Task<IResult> UpdatePassword(string userId, ChangePasswordRequest request, 
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId + "");
            if (user == null) return Result.Fail(ResultStatus.Unauthorized);
            // TEMPORARY UNTIL EMAIL CONFIRMATION IS SETUP
            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            return result.ToResult();
        }

        public async Task<IResult> UpdateUser(string userId, UpdateUserDetailsRequest request, 
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId + "");
            if (user == null) return Result.Fail(ResultStatus.Unauthorized);
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