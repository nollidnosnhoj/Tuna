using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Users.Models;

namespace Audiochan.Core.Interfaces
{
    public interface IUserService
    {
        Task<IResult<CurrentUserViewModel>> GetCurrentUser(long authUserId, CancellationToken cancellationToken = default);
        Task<IResult<UserDetailsViewModel>> GetUserDetails(string username,
            CancellationToken cancellationToken = default);

        Task<IResult> UpdateUsername(long userId, string newUsername, CancellationToken cancellationToken = default);
        Task<IResult> UpdateEmail(long userId, string newEmail, CancellationToken cancellationToken = default);
        Task<IResult> UpdatePassword(long userId, string newPassword, CancellationToken cancellationToken = default);
        Task<IResult> UpdateUser(long userId, UpdateUserDetailsRequest request, CancellationToken cancellationToken = default);
        Task<bool> CheckIfUsernameExists(string username, CancellationToken cancellationToken = default);
        Task<bool> CheckIfEmailExists(string email, CancellationToken cancellationToken = default);
    }
}