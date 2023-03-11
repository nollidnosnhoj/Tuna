using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Auth.Models;
using Audiochan.Core.Features.Auth.Results;

namespace Audiochan.Core.Features.Auth;

public interface IAuthService
{
    Task<LoginResult> LoginWithPasswordAsync(string login, string password, string? ipAddress, CancellationToken cancellationToken = default);
    Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken,  string? ipAddress, CancellationToken cancellationToken = default);
    Task<RevokeRefreshTokenResult> RevokeRefreshTokenAsync(string refreshToken, string? ipAddress, CancellationToken? cancellationToken = default);
}