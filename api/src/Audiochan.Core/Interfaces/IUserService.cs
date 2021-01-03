using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Profiles.Models;
using Audiochan.Core.Features.Users.Models;

namespace Audiochan.Core.Interfaces
{
    public interface IUserService
    {
        Task<IResult<CurrentUserViewModel>> GetCurrentUser(CancellationToken cancellationToken = default);
        Task<bool> CheckIfUsernameExists(string username, CancellationToken cancellationToken = default);
        Task<bool> CheckIfEmailExists(string email, CancellationToken cancellationToken = default);
    }
}