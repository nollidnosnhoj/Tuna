using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Auth.Models;
using Audiochan.Core.Features.Users.Models;

namespace Audiochan.Core.Interfaces
{
    public interface IAuthService
    {
        Task<IResult<AuthResultDto>> Login(string username, string password, CancellationToken cancellationToken = default);
        Task<IResult> Register(CreateUserRequest request, CancellationToken cancellationToken = default);
        Task<IResult<AuthResultDto>> Refresh(string? refreshToken, CancellationToken cancellationToken = default);
        Task<IResult> Revoke(string? refreshToken, CancellationToken cancellationToken = default);
    }
}