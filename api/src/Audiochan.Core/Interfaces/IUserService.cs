using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Users.Models;

namespace Audiochan.Core.Interfaces
{
    public interface IUserService
    {
        Task<IResult<CurrentUserViewModel>> GetCurrentUser(string authUserId, CancellationToken cancellationToken = default);
        Task<IResult<UserDetailsViewModel>> GetUserDetails(string username,
            CancellationToken cancellationToken = default);
        Task<IResult> UpdateUsername(string userId, string newUsername, CancellationToken cancellationToken = default);
        Task<IResult> UpdateEmail(string userId, string newEmail, CancellationToken cancellationToken = default);
        Task<IResult> UpdatePassword(string userId, ChangePasswordRequest request, 
            CancellationToken cancellationToken = default);
        Task<IResult> UpdateUser(string userId, UpdateUserDetailsRequest request, CancellationToken cancellationToken = default);
        Task<bool> CheckIfUsernameExists(string username, CancellationToken cancellationToken = default);
        Task<bool> CheckIfEmailExists(string email, CancellationToken cancellationToken = default);
    }
}